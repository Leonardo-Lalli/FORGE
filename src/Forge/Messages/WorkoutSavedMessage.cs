using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Forge.Messages;

public class WorkoutSavedMessage : ValueChangedMessage<bool>
{
    public WorkoutSavedMessage() : base(true) { }
}
