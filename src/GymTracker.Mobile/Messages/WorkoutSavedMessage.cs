using CommunityToolkit.Mvvm.Messaging.Messages;

namespace GymTracker.Mobile.Messages;

public class WorkoutSavedMessage : ValueChangedMessage<bool>
{
    public WorkoutSavedMessage() : base(true) { }
}
