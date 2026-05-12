using System.Globalization;

namespace GymTracker.Mobile.Converters;

public class DateTimeFormatConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is DateTime dt)
        {
            var format = parameter as string ?? "dd/MM/yyyy HH:mm";
            return dt.ToString(format, culture);
        }
        return string.Empty;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
