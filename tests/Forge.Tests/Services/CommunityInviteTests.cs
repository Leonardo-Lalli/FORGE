using Forge.Services;
namespace Forge.Tests.Services;
public class CommunityInviteTests
{
    [Fact] public void Default_invite_uses_public_community_and_website()
    {
        Assert.Contains("predefinito", CommunityInvite.Message(""));
        Assert.EndsWith("#download", CommunityInvite.DownloadUrl);
    }
    [Fact] public void Personal_invite_explains_manual_server_selection()
    {
        Assert.Contains("https://gym.example", CommunityInvite.Message("gym.example"));
        Assert.DoesNotContain("predefinito", CommunityInvite.Message("gym.example"));
        Assert.Throws<ArgumentException>(() => CommunityInvite.Message("https://user:password@gym.example"));
    }
}
