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
