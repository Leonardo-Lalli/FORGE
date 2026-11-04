using System.Globalization;
using System.Text.Json;
using CommunityToolkit.Mvvm.Messaging;
using Forge.Messages;
using Forge.Services;

namespace Forge.Tests.Services;

[Collection("Account")]
public class WeightUnitTests : IDisposable
{
    private readonly string original = Preferences.Get("weight_unit", "kg");
    public WeightUnitTests() => Preferences.Set("weight_unit", "kg");
    public void Dispose() => Preferences.Set("weight_unit", original);

    [Fact]
    public void PoundsPreferencePersistsAndUsesExactConversion()
    {
        var units = new WeightUnitService { UsePounds = true };
        Assert.Equal(220.46226218487757, units.ToDisplay(100), 10);
        Assert.Equal(45.359237, units.ToKilograms(100), 10);
        Assert.True(new WeightUnitService().UsePounds);
        Assert.Equal("lb", units.Symbol);
        Assert.Equal("WeightLb", units.CsvHeader);
    }
    [Fact]
    public void ChangingUnitNotifiesCachedScreens()
    {
        var recipient = new object();
        var messages = 0;
        WeakReferenceMessenger.Default.Register<WeightUnitChangedMessage>(recipient, (_, _) => messages++);
        try
        {
            var units = new WeightUnitService();
            units.UsePounds = true; units.UsePounds = false;
            Assert.Equal(2, messages);
        }
        finally { WeakReferenceMessenger.Default.UnregisterAll(recipient); }
    }
    [Fact]
    public void RepeatedSwitchesAndEntryWritebackNeverAlterCanonicalWeightOrSuggestions()
    {
        var units = new WeightUnitService();
        var set = new ExerciseSet { WeightKg = 100, Reps = 8, IsSuggested = true };
        set.SetPrevious(90, 8);
        for (var i = 0; i < 100; i++)
        {
            units.UsePounds = !units.UsePounds;
            set.ConfigureUnits(units);
            set.DisplayWeight = set.DisplayWeight; // MAUI can write back the displayed rounded value.
            Assert.Equal(100, set.WeightKg);
            Assert.True(set.IsSuggested);
        }
        units.UsePounds = true; set.ConfigureUnits(units);
        Assert.Equal(220.46, set.DisplayWeight);
        Assert.Contains("lb", set.PreviousDisplay);
        set.DisplayWeight = 100;
        Assert.Equal(45.359237, set.WeightKg, 10);
        Assert.False(set.IsSuggested);
    }
    [Fact]
    public void PlanAndDraftJsonKeepKgAndExcludeDisplayProjections()
    {
        var set = new ExerciseSet { WeightKg = 100 };
        set.ConfigureUnits(new WeightUnitService { UsePounds = true });
        var json = JsonSerializer.Serialize(set);
        Assert.Contains("\"WeightKg\":100", json);
        Assert.DoesNotContain("DisplayWeight", json);
        Assert.DoesNotContain("PreviousDisplay", json);
        Assert.Equal(100, JsonSerializer.Deserialize<ExerciseSet>(json)!.WeightKg);
    }
    [Fact]
    public void ExportAndImportWeightsHaveExplicitUnitsIndependentOfCurrentSetting()
    {
        var workout = new LocalWorkout { Name = "Routine", Date = "2026-10-26", Notes = "Notes", ExerciseDataJson = "[{\"name\":\"Squat\",\"sets\":[{\"weightKg\":100,\"reps\":8,\"isCompleted\":true}]}]" };
        var units = new WeightUnitService { UsePounds = true };
        var csv = CsvWorkoutWriter.Write([workout], units);
        Assert.Contains("WeightLb", csv);
        var displayed = double.Parse(csv.Split('\n')[1].Split(',')[5], CultureInfo.InvariantCulture);
        Assert.Equal(100, WeightUnitService.CsvToKilograms(displayed, "WeightLb"), 7);
        units.UsePounds = false;
        Assert.Equal(100, WeightUnitService.CsvToKilograms(displayed, "WeightLb"), 7);
        Assert.Contains("WeightKg", CsvWorkoutWriter.Write([workout], units));
        Assert.Equal(100, WeightUnitService.CsvToKilograms(100, "WeightKg"));
    }
    [Fact]
    public void ExportPreservesLegacySummariesAndExcludesDrafts()
    {
        var csv = CsvWorkoutWriter.Write([new() { Name = "Legacy", ExercisesJson = "[\"Squat\"]" }, new() { Name = "Draft", IsDraft = true }], new WeightUnitService());
        Assert.Contains("Legacy", csv);
        Assert.Contains("Squat", csv);
        Assert.DoesNotContain("Draft", csv);
    }

    [Fact]
    public void CsvKeepsMultilineNotesAndQuotedValuesInOneRecord()
    {
        var csv = CsvWorkoutWriter.Write([new LocalWorkout { Name = "Workout, A", Notes = "First\nSecond \"quoted\"", ExerciseDataJson = "[{\"name\":\"Squat\",\"sets\":[{\"weightKg\":20,\"reps\":8}]}]" }], new WeightUnitService());
        var rows = CsvWorkoutWriter.SplitRows(csv);
        Assert.Equal(2, rows.Length);
        Assert.Contains("First\nSecond \"\"quoted\"\"", rows[1]);
        Assert.Equal("First\nSecond \"quoted\"", CsvWorkoutWriter.ParseRow(rows[1])[6]);
        Assert.Throws<FormatException>(() => CsvWorkoutWriter.SplitRows("a,\"unclosed"));
    }

    [Fact]
    public void VolumesAndAchievementThresholdsConvertWithoutChangingUnlockCriteria()
    {
        var units = new WeightUnitService { UsePounds = true };
        Assert.Contains("lb", units.FormatCompact(10000));
        Assert.DoesNotContain("kg", units.AchievementDescription("club_100", "100 kg"));
        var display = new Forge.ViewModels.AchievementDisplay { Id = "max_weight", Progress = 500, MaxProgress = 1000 };
        display.ConfigureUnits(units);
        Assert.Contains("lb", display.ProgressText);
        Assert.Equal(0.5, display.ProgressPercent);
        Assert.Equal(1000, display.MaxProgress);
    }
}
