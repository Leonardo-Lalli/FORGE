using System.Globalization;

// DatabaseService tests always inject a unique database path.
internal static class FileSystem
{
    public static string AppDataDirectory => Path.GetTempPath();
    public static string CacheDirectory => Path.GetTempPath();
    public static Task<Stream> OpenAppPackageFileAsync(string path) => throw new FileNotFoundException();
}

internal static class Preferences
{
    private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, string> values = new();
    public static string Get(string key, string fallback) => values.GetValueOrDefault(key, fallback);
    public static bool Get(string key, bool fallback) => bool.TryParse(Get(key, ""), out var value) ? value : fallback;
    public static int Get(string key, int fallback) => int.TryParse(Get(key, ""), out var value) ? value : fallback;
    public static void Set(string key, string value) => values[key] = value;
    public static void Set(string key, bool value) => Set(key, value.ToString());
    public static void Set(string key, int value) => Set(key, value.ToString());
    public static void Remove(string key) => values.TryRemove(key, out _);
}

internal static class SecureStorage
{
    private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, string> values = new();
    public static Task<string?> GetAsync(string key) => Task.FromResult(values.GetValueOrDefault(key));
    public static Task SetAsync(string key, string value) { values[key] = value; return Task.CompletedTask; }
    public static void Remove(string key) => values.TryRemove(key, out _);
}

internal sealed class Shell
{
    public static Shell Current { get; } = new();
    public Task GoToAsync(string route) => Task.CompletedTask;
}

namespace Microsoft.Maui.Controls
{
    public class ImageSource
    {
        public static ImageSource FromFile(string path) => new();
        public static ImageSource FromUri(Uri uri) => new();
        public static ImageSource FromStream(Func<Stream> stream) => new();
    }
    public interface IValueConverter
    {
        object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture);
        object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture);
    }
}

namespace Microsoft.Maui
{
    [AttributeUsage(AttributeTargets.All)]
    public class AcceptEmptyServiceProviderAttribute : Attribute { }
}
