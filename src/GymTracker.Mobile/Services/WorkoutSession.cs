namespace GymTracker.Mobile.Services;

public class WorkoutSession
{
    public bool IsActive { get; private set; }
    public bool IsMinimized { get; private set; }
    public string PlanName { get; private set; } = string.Empty;
    public string ElapsedTime { get; private set; } = "00:00:00";
    public string RestTimerText { get; private set; } = string.Empty;
    public bool IsRestTimerActive { get; private set; }

    public event Action? StateChanged;

    public void Start(string planName)
    {
        IsActive = true;
        IsMinimized = false;
        PlanName = planName;
        StateChanged?.Invoke();
    }

    public void Minimize()
    {
        IsMinimized = true;
        StateChanged?.Invoke();
    }

    public void Restore()
    {
        IsMinimized = false;
        StateChanged?.Invoke();
    }

    public void UpdateElapsed(string elapsed)
    {
        ElapsedTime = elapsed;
        StateChanged?.Invoke();
    }

    public void UpdateRestTimer(bool active, string text)
    {
        IsRestTimerActive = active;
        RestTimerText = text;
        StateChanged?.Invoke();
    }

    public void End()
    {
        IsActive = false;
        IsMinimized = false;
        PlanName = string.Empty;
        ElapsedTime = "00:00:00";
        RestTimerText = string.Empty;
        IsRestTimerActive = false;
        StateChanged?.Invoke();
    }
}
