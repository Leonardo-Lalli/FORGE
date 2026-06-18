using Forge.Messages;
using Forge.Models.Dto;
using System.Text.Json;

namespace Forge.Tests.Integration;

public class WorkoutSavedMessageTests
{
    [Fact]
    public void Message_DefaultValue_IsTrue()
    {
        var msg = new WorkoutSavedMessage();
        Assert.True(msg.Value);
    }
}

public class PocketBaseDtoTests
{
    [Fact]
    public void LoggedWorkoutRecord_Defaults_AreEmpty()
    {
        var record = new LoggedWorkoutRecord();
        Assert.Equal("", record.Id);
        Assert.Equal("", record.Name);
        Assert.Equal(0, record.Volume);
        Assert.Equal(0, record.Duration);
        Assert.Empty(record.Exercises);
        Assert.Empty(record.LikedBy);
        Assert.Empty(record.Photos);
    }

    [Fact]
    public void LoggedWorkoutRecord_Photos_CanBePopulated()
    {
        var record = new LoggedWorkoutRecord
        {
            Photos = new List<string> { "base64photo1", "base64photo2" }
        };
        Assert.Equal(2, record.Photos.Count);
    }

    [Fact]
    public void PocketBaseUserRecord_Defaults_AreEmpty()
    {
        var user = new PocketBaseUserRecord();
        Assert.Equal("", user.Id);
        Assert.Equal("", user.Email);
        Assert.Equal("", user.Name);
    }

    [Fact]
    public void SocialGraphRecord_Status_Works()
    {
        var record = new SocialGraphRecord
        {
            Status = "pending",
            FromUser = "user1",
            ToUser = "user2"
        };
        Assert.Equal("pending", record.Status);
        Assert.Equal("user1", record.FromUser);
        Assert.Equal("user2", record.ToUser);
    }
}
