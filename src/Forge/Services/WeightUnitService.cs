using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Forge.Messages;

namespace Forge.Services;

public partial class WeightUnitService : ObservableObject
{
    public const double KilogramsPerPound = 0.45359237;
    [ObservableProperty, NotifyPropertyChangedFor(nameof(Symbol))] private bool usePounds = Preferences.Get("weight_unit", "kg") == "lb";
    public string Symbol => UsePounds ? "lb" : "kg";
    public double ToDisplay(double kilograms) => UsePounds ? kilograms / KilogramsPerPound : kilograms;
    public double ToKilograms(double displayed) => UsePounds ? displayed * KilogramsPerPound : displayed;
    public string Format(double kilograms, string format = "0.##") => $"{ToDisplay(kilograms).ToString(format)} {Symbol}";
    public string CsvHeader => UsePounds ? "WeightLb" : "WeightKg";
    public string FormatCompact(double kilograms) => ToDisplay(kilograms) >= 1000
        ? $"{ToDisplay(kilograms) / 1000:0.#}k {Symbol}" : Format(kilograms);
    public static double CsvToKilograms(double value, string header) => header.Trim().ToLowerInvariant() is "weightlb" or "weightlbs" or "pesolb"
        ? value * KilogramsPerPound : value;
    public string AchievementProgress(string id, int progress, int maximum) => id is "max_weight" or "iron_marathon" or "ton_week"
        ? $"{ToDisplay(progress):N0}/{Format(maximum, "N0")}" : $"{progress}/{maximum}";
    partial void OnUsePoundsChanged(bool value)
    {
        Preferences.Set("weight_unit", value ? "lb" : "kg");
        WeakReferenceMessenger.Default.Send(new WeightUnitChangedMessage());
    }
    public string AchievementDescription(string id, string original) => id switch
    {
        "max_weight" => $"Solleva {Format(1000, "N0")} in una sessione",
        "club_100" => $"Esegui una serie con {Format(100, "0.##")}",
        "iron_marathon" => $"Solleva {Format(10000, "N0")} in una settimana",
        "ton_week" => $"Solleva {Format(50000, "N0")} in un mese",
        _ => original
    };
}
