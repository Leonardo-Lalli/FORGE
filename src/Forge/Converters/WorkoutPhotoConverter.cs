using System.Globalization;

namespace Forge.Converters;

public class WorkoutPhotoConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var source = value as string;
        if (string.IsNullOrWhiteSpace(source)) return null;
        if (!source.StartsWith("data:image/", StringComparison.OrdinalIgnoreCase)) return Uri.TryCreate(source, UriKind.Absolute, out var uri) && uri.Scheme is "https" or "http" ? ImageSource.FromUri(uri) : ImageSource.FromFile(source);
        try
        {
            var comma = source.IndexOf(',');
            var bytes = System.Convert.FromBase64String(source[(comma + 1)..]);
            return ImageSource.FromStream(() => new MemoryStream(bytes, writable: false));
        }
        catch (FormatException) { return null; }
    }
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();
}
