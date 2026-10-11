using Forge.Services;
namespace Forge.Tests;
public class ServerAddressTests
{
    [Theory]
    [InlineData(null, "https://leoforge.duckdns.org")]
    [InlineData("", "https://leoforge.duckdns.org")]
    [InlineData("http://leoforge.duckdns.org/", "https://leoforge.duckdns.org")]
    [InlineData("https://example.org/api/", "https://example.org")]
    [InlineData("http://192.168.1.50:8090", "http://192.168.1.50:8090")]
    public void NormalizesValidServer(string? value, string expected)
    { Assert.True(ServerAddress.TryNormalize(value, out var actual)); Assert.Equal(expected, actual); }
    [Theory]
    [InlineData("file:///tmp/server")]
    [InlineData("https://user:password@example.org")]
    [InlineData("https://example.org?token=secret")]
    public void RejectsInvalidServer(string value)
    { Assert.False(ServerAddress.TryNormalize(value, out _)); }
}
