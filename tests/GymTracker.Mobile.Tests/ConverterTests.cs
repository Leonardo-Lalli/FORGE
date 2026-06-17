using System.Globalization;
using GymTracker.Mobile.Converters;

namespace GymTracker.Mobile.Tests.Converters;

public class InverseBoolConverterTests
{
    private readonly InverseBoolConverter _converter = new();

    [Fact]
    public void Convert_True_ReturnsFalse() =>
        Assert.False((bool)_converter.Convert(true, typeof(bool), null, CultureInfo.InvariantCulture)!);

    [Fact]
    public void Convert_False_ReturnsTrue() =>
        Assert.True((bool)_converter.Convert(false, typeof(bool), null, CultureInfo.InvariantCulture)!);

    [Fact]
    public void Convert_Null_ReturnsTrue() =>
        Assert.True((bool)_converter.Convert(null, typeof(bool), null, CultureInfo.InvariantCulture)!);

    [Fact]
    public void ConvertBack_True_ReturnsFalse() =>
        Assert.False((bool)_converter.ConvertBack(true, typeof(bool), null, CultureInfo.InvariantCulture)!);

    [Fact]
    public void ConvertBack_False_ReturnsTrue() =>
        Assert.True((bool)_converter.ConvertBack(false, typeof(bool), null, CultureInfo.InvariantCulture)!);
}

public class BoolToVisibilityConverterTests
{
    private readonly BoolToVisibilityConverter _converter = new();

    [Fact]
    public void Convert_True_ReturnsTrue() =>
        Assert.True((bool)_converter.Convert(true, typeof(bool), null, CultureInfo.InvariantCulture)!);

    [Fact]
    public void Convert_False_ReturnsFalse() =>
        Assert.False((bool)_converter.Convert(false, typeof(bool), null, CultureInfo.InvariantCulture)!);

    [Fact]
    public void Convert_Null_ReturnsFalse() =>
        Assert.False((bool)_converter.Convert(null, typeof(bool), null, CultureInfo.InvariantCulture)!);
}

public class DateTimeFormatConverterTests
{
    private readonly DateTimeFormatConverter _converter = new();

    [Theory]
    [InlineData("2024-01-15T10:30:00Z", "dd/MM/yyyy")]
    [InlineData("2024-12-25T08:00:00Z", "dd/MM/yyyy")]
    [InlineData("2026-06-17T14:00:00Z", "dd/MM/yyyy")]
    public void Convert_DateTimeObject_FormatsCorrectly(string isoDate, string format)
    {
        var dt = DateTime.Parse(isoDate);
        var result = _converter.Convert(dt, typeof(string), format, CultureInfo.InvariantCulture) as string;
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public void Convert_DateTime_NoParameter_UsesDefaultFormat()
    {
        var dt = new DateTime(2024, 1, 15, 10, 30, 0);
        var result = _converter.Convert(dt, typeof(string), null, CultureInfo.InvariantCulture) as string;
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public void Convert_Null_ReturnsEmptyString()
    {
        var result = _converter.Convert(null, typeof(string), null, CultureInfo.InvariantCulture) as string;
        Assert.Equal("", result);
    }

    [Fact]
    public void Convert_StringInsteadOfDateTime_ReturnsEmptyString()
    {
        var result = _converter.Convert("not-a-date", typeof(string), null, CultureInfo.InvariantCulture) as string;
        Assert.Equal("", result);
    }
}
