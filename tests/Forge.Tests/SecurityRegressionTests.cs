using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using Forge.Services;

namespace Forge.Tests;

[CollectionDefinition("Account", DisableParallelization = true)]
public class AccountCollection { }

[Collection("Account")]
public class SecurityRegressionTests
{
    [Fact]
    public async Task CertificateWithCopiedLetsEncryptNameIsRejected()
    {
        using var rsa = RSA.Create(2048);
        var request = new CertificateRequest("CN=ISRG Root X1", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        var names = new SubjectAlternativeNameBuilder(); names.AddDnsName("localhost");
        request.CertificateExtensions.Add(names.Build());
        using var certificate = request.CreateSelfSigned(DateTimeOffset.UtcNow.AddMinutes(-1), DateTimeOffset.UtcNow.AddDays(1));
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(15));
        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        var server = Task.Run(async () =>
        {
            try
            {
                using var socket = await listener.AcceptTcpClientAsync(timeout.Token);
                using var tls = new SslStream(socket.GetStream());
                await tls.AuthenticateAsServerAsync(new SslServerAuthenticationOptions { ServerCertificate = certificate }, timeout.Token);
                var buffer = new byte[4096];
                await tls.ReadAsync(buffer, timeout.Token);
                await tls.WriteAsync(Encoding.ASCII.GetBytes("HTTP/1.1 200 OK\r\nContent-Length: 2\r\nConnection: close\r\n\r\nOK"), timeout.Token);
            }
            catch (Exception ex) when (ex is IOException or System.Security.Authentication.AuthenticationException or OperationCanceledException) { }
        });
        using var handler = new CertificatePinningHandler { UseProxy = false };
        using var client = new HttpClient(handler);
        await Assert.ThrowsAsync<HttpRequestException>(() => client.GetAsync($"https://localhost:{port}/", timeout.Token));
        timeout.Cancel();
        await server;
    }

    [Fact]
    public async Task RefreshSendsExistingTokenAndRetriesWithNewToken()
    {
        var calls = new List<(string Path, string? Token)>();
        var factory = new FakeFactory(req =>
        {
            var path = req.RequestUri!.AbsolutePath;
            var token = req.Headers.Authorization?.Parameter;
            calls.Add((path, token));
            if (path.EndsWith("auth-with-password")) return Json("{\"token\":\"old-token\",\"record\":{\"id\":\"alice\"}}");
            if (path.EndsWith("auth-refresh")) return Json("{\"token\":\"new-token\",\"record\":{\"id\":\"alice\"}}");
            if (token == "old-token") return new(HttpStatusCode.Unauthorized);
            return Json("{\"items\":[]}");
        });
        var pb = Service(factory);
        Assert.True((await pb.LoginAsync("test@example.invalid", "test-password")).Success);
        await pb.GetMyWorkoutsAsync();
        Assert.Contains(calls, c => c.Path.EndsWith("auth-refresh") && c.Token == "old-token");
        Assert.Equal("new-token", calls[^1].Token);
        Assert.Equal(4, calls.Count);
    }

    [Fact]
    public async Task AvatarUrlDoesNotExposeLoginTokenAndSwitchServerRemovesCredentials()
    {
        var pb = Service(new FakeFactory(_ => Json("{\"token\":\"secret-token\",\"record\":{\"id\":\"alice\"}}")));
        await pb.LoginAsync("test@example.invalid", "test-password");
        var url = pb.GetFileUrl("users", "alice", "photo space.jpg");
        Assert.DoesNotContain("secret-token", url);
        Assert.DoesNotContain("?token=", url);
        Assert.Contains("photo%20space.jpg", url);
        pb.InvalidateClient();
        Assert.False(pb.IsLoggedIn);
        Assert.Null(await SecureStorage.GetAsync("pb_password"));
        Assert.Null(await SecureStorage.GetAsync("pb_email"));
    }

    [Fact]
    public async Task InvalidJsonAndTransportFailureProduceSafeLoginErrors()
    {
        var pb = Service(new FakeFactory(_ => Json("malformed-private-server-data")));
        var badJson = await pb.LoginAsync("test@example.invalid", "password");
        Assert.False(badJson.Success);
        Assert.DoesNotContain("private", badJson.Error);
        pb = Service(new FakeFactory(_ => throw new HttpRequestException("internal host details")));
        Assert.False((await pb.LoginAsync("test@example.invalid", "password")).Success);
    }

    [Fact]
    public async Task FailedPostChecksStableIdAndNeverUploadsOtherUsersOrDrafts()
    {
        var calls = new List<string>();
        var pb = Service(new FakeFactory(req =>
        {
            calls.Add(req.Method + " " + req.RequestUri!.AbsolutePath);
            if (req.RequestUri.AbsolutePath.EndsWith("auth-with-password")) return Json("{\"token\":\"token\",\"record\":{\"id\":\"alice\"}}");
            if (req.Method == HttpMethod.Post) return new(HttpStatusCode.BadRequest);
            return Json("{\"id\":\"abcdefghijk1234\",\"user\":\"alice\"}");
        }));
        await pb.LoginAsync("test@example.invalid", "password");
        Assert.False(await pb.UploadWorkoutAsync(new() { Id = "other", UserId = "bob" }));
        Assert.False(await pb.UploadWorkoutAsync(new() { Id = "draft", UserId = "alice", IsDraft = true }));
        Assert.Single(calls);
        Assert.True(await pb.UploadWorkoutAsync(new() { Id = "abcdefghijk1234", UserId = "alice" }));
        Assert.Equal("GET /api/collections/logged_workouts/records/abcdefghijk1234", calls[^1]);
    }

    [Fact]
    public async Task AchievementsIgnoreOtherAccountsAndHaveRatioProgress()
    {
        var path = Path.Combine(Path.GetTempPath(), $"forge-account-{Guid.NewGuid():N}.db");
        try
        {
            var db = new DatabaseService(path);
            await db.SaveWorkoutAsync(new() { Id = "bob", UserId = "bob", Date = DateTime.Now.ToString("O") });
            var pb = Service(new FakeFactory(_ => Json("{\"token\":\"token\",\"record\":{\"id\":\"alice\"}}")));
            await pb.LoginAsync("test@example.invalid", "password");
            var service = new AchievementService(db, pb);
            Assert.Equal(0, await service.GetUnlockedCountAsync());
            Assert.Equal(0, (await service.GetTrainingProgressAsync()).Experience);
            await db.SaveWorkoutAsync(new() { Id = "alice", UserId = "alice", Date = DateTime.Now.ToString("O"), Duration = 60 });
            Assert.True(await service.GetUnlockedCountAsync() > 0);
            Assert.InRange(await service.GetProgressPercentAsync(), 0.001, 1);
            Assert.Equal(100, (await service.GetTrainingProgressAsync()).Experience);
        }
        finally { await new SQLite.SQLiteAsyncConnection(path, SQLite.SQLiteOpenFlags.ReadWrite | SQLite.SQLiteOpenFlags.Create | SQLite.SQLiteOpenFlags.SharedCache).CloseAsync(); File.Delete(path); }
    }

    [Fact]
    public async Task NoCustomUrlLogsInThroughPublicHttpsServer()
    {
        Preferences.Remove("pb_server_url");
        Uri? address = null;
        var pb = new PocketBaseService(new FakeFactory(req =>
        {
            address = req.RequestUri;
            return Json("{\"token\":\"token\",\"record\":{\"id\":\"test\"}}");
        }), new BuildSecrets());
        Assert.True((await pb.LoginAsync("test@example.invalid", "password")).Success);
        Assert.Equal("https://leoforge.duckdns.org/api/collections/users/auth-with-password", address!.AbsoluteUri);
        pb.Logout();
    }

    [Fact]
    public async Task RegistrationCreatesUserThenAuthenticates()
    {
        var paths = new List<string>();
        var pb = Service(new FakeFactory(req =>
        {
            paths.Add(req.RequestUri!.AbsolutePath);
            if (paths.Count == 1)
            {
                var payload = JsonDocument.Parse(req.Content!.ReadAsStringAsync().GetAwaiter().GetResult());
                Assert.Equal("Athlete", payload.RootElement.GetProperty("name").GetString());
                Assert.Equal("password", payload.RootElement.GetProperty("passwordConfirm").GetString());
                return Json("{\"id\":\"test\"}");
            }
            return Json("{\"token\":\"token\",\"record\":{\"id\":\"test\"}}");
        }));
        Assert.True((await pb.RegisterAsync("test@example.invalid", "password", "Athlete")).Success);
        Assert.Equal(new[] { "/api/collections/users/records", "/api/collections/users/auth-with-password" }, paths);
        pb.Logout();
    }
    private static PocketBaseService Service(FakeFactory factory)
    {
        Preferences.Set("pb_server_url", "https://example.invalid");
        return new(factory, new BuildSecrets());
    }
    private static HttpResponseMessage Json(string body) => new(HttpStatusCode.OK) { Content = new StringContent(body, Encoding.UTF8, "application/json") };
    private sealed class FakeFactory(Func<HttpRequestMessage, HttpResponseMessage> respond) : IHttpClientFactory
    {
        public HttpClient CreateClient(string name) => new(new Handler(respond));
    }
    private sealed class Handler(Func<HttpRequestMessage, HttpResponseMessage> respond) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken token) => Task.FromResult(respond(request));
    }
}


