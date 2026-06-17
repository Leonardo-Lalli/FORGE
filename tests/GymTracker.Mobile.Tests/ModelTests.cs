using System.Collections.ObjectModel;
using System.Text.Json;
using GymTracker.Mobile.Models;
using GymTracker.Mobile.Models.Dto;

namespace GymTracker.Mobile.Tests.Models;

public class ExerciseSetTests
{
    [Fact]
    public void Constructor_Defaults_IdIsNotEmpty() =>
        Assert.NotEmpty(new ExerciseSet().Id);

    [Fact]
    public void WeightKg_SetAndGet_Works()
    {
        var set = new ExerciseSet();
        set.WeightKg = 80.5;
        Assert.Equal(80.5, set.WeightKg);
    }

    [Fact]
    public void Reps_SetAndGet_Works()
    {
        var set = new ExerciseSet();
        set.Reps = 12;
        Assert.Equal(12, set.Reps);
    }

    [Fact]
    public void IsCompleted_SetTrue_ReflectsCorrectly()
    {
        var set = new ExerciseSet { IsCompleted = true };
        Assert.True(set.IsCompleted);
    }
}

public class WorkoutExerciseTests
{
    [Fact]
    public void Constructor_Defaults_SetsIsEmpty()
    {
        var ex = new WorkoutExercise();
        Assert.NotNull(ex.Sets);
        Assert.Empty(ex.Sets);
    }

    [Fact]
    public void Constructor_Defaults_IdIsNotEmpty()
    {
        var ex = new WorkoutExercise();
        Assert.NotEmpty(ex.Id);
    }

    [Fact]
    public void ExerciseName_SetAndGet_Works()
    {
        var ex = new WorkoutExercise { ExerciseName = "Bench Press" };
        Assert.Equal("Bench Press", ex.ExerciseName);
    }
}

public class WorkoutPlanTests
{
    [Fact]
    public void Constructor_Defaults_ExercisesIsEmpty()
    {
        var plan = new WorkoutPlan();
        Assert.NotNull(plan.Exercises);
        Assert.Empty(plan.Exercises);
    }

    [Fact]
    public void Constructor_Defaults_RestSecondsIs90()
    {
        var plan = new WorkoutPlan();
        Assert.Equal(90, plan.RestSeconds);
    }

    [Fact]
    public void Name_SetAndGet_Works()
    {
        var plan = new WorkoutPlan { Name = "Push Day" };
        Assert.Equal("Push Day", plan.Name);
    }
}

public class ExerciseDbV1DtoTests
{
    [Fact]
    public void GetBodyParts_FromJsonArray_ReturnsList()
    {
        var json = """{"exerciseId":"test1","name":"Test","bodyParts":["chest","shoulders"],"equipments":["barbell"],"targetMuscles":["pectorals"],"secondaryMuscles":["triceps"],"instructions":["Step 1","Step 2"],"gifUrl":"http://example.com/test.gif"}""";
        var dto = JsonSerializer.Deserialize<ExerciseDbV1Dto>(json)!;

        var parts = dto.GetBodyParts();
        Assert.Equal(2, parts.Count);
        Assert.Contains("chest", parts);
        Assert.Contains("shoulders", parts);
    }

    [Fact]
    public void GetBodyParts_FromJsonString_ReturnsSingleItemList()
    {
        var json = """{"exerciseId":"test2","name":"Test","bodyParts":"chest","equipments":"barbell","targetMuscles":"pectorals","secondaryMuscles":"triceps shoulders","instructions":["Step"],"gifUrl":"http://example.com/test.gif"}""";
        var dto = JsonSerializer.Deserialize<ExerciseDbV1Dto>(json)!;

        var parts = dto.GetBodyParts();
        Assert.Single(parts);
        Assert.Equal("chest", parts[0]);
    }

    [Fact]
    public void GetEquipments_FromJsonArray_ReturnsList()
    {
        var json = """{"exerciseId":"test3","name":"Test","bodyParts":["chest"],"equipments":["barbell","bench"],"targetMuscles":["pectorals"],"secondaryMuscles":[],"instructions":[],"gifUrl":"http://example.com/test.gif"}""";
        var dto = JsonSerializer.Deserialize<ExerciseDbV1Dto>(json)!;

        var eq = dto.GetEquipments();
        Assert.Equal(2, eq.Count);
    }

    [Fact]
    public void GetInstructions_FromJsonArray_ReturnsList()
    {
        var json = """{"exerciseId":"test4","name":"Test","bodyParts":["chest"],"equipments":["barbell"],"targetMuscles":["pectorals"],"secondaryMuscles":[],"instructions":["Step 1: Setup","Step 2: Execute"],"gifUrl":"http://example.com/test.gif"}""";
        var dto = JsonSerializer.Deserialize<ExerciseDbV1Dto>(json)!;

        var inst = dto.GetInstructions();
        Assert.Equal(2, inst.Count);
        Assert.Equal("Step 1: Setup", inst[0]);
    }

    [Fact]
    public void GetSecondaryMuscles_SpaceSeparated_ReturnsList()
    {
        var json = """{"exerciseId":"test5","name":"Test","bodyParts":"chest","equipments":"barbell","targetMuscles":"pectorals","secondaryMuscles":"triceps shoulders","instructions":[],"gifUrl":"http://example.com/test.gif"}""";
        var dto = JsonSerializer.Deserialize<ExerciseDbV1Dto>(json)!;

        var sec = dto.GetSecondaryMuscles();
        Assert.Equal(2, sec.Count);
        Assert.Contains("triceps", sec);
        Assert.Contains("shoulders", sec);
    }

    [Fact]
    public void Deserialize_NullFields_GetEmptyLists()
    {
        var json = """{"exerciseId":"test6","name":"Test","gifUrl":"http://example.com/test.gif"}""";
        var dto = JsonSerializer.Deserialize<ExerciseDbV1Dto>(json)!;

        Assert.Empty(dto.GetBodyParts());
        Assert.Empty(dto.GetEquipments());
        Assert.Empty(dto.GetInstructions());
    }

    [Fact]
    public void ExerciseDbV1ListResponse_Deserialization_Works()
    {
        var json = """{"success":true,"data":[{"exerciseId":"abc","name":"Push Up","bodyParts":["chest"],"equipments":["body weight"],"gifUrl":"http://example.com/pushup.gif","targetMuscles":["pectorals"],"secondaryMuscles":["triceps"],"instructions":["Step 1"]}],"meta":{"total":1,"hasNextPage":false,"nextCursor":""}}""";
        var response = JsonSerializer.Deserialize<ExerciseDbV1ListResponse>(json)!;

        Assert.True(response.Success);
        Assert.Single(response.Data);
        Assert.Equal("Push Up", response.Data[0].Name);
        Assert.Equal("abc", response.Data[0].ExerciseId);
        Assert.False(response.Meta?.HasNextPage);
    }
}
