using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Forge.Services;

public partial class WorkoutSession : ObservableObject
{
    private readonly DatabaseService db;
    private readonly PocketBaseService pb;
    public WorkoutSession(DatabaseService db, PocketBaseService pb) { this.db = db; this.pb = pb; }
    public Func<Task>? SuspendHandler { get; set; }
    public Func<Task>? ForegroundHandler { get; set; }
    public bool ShowMiniBar => IsActive && IsMinimized;
    public bool IsSuspended { get; private set; }
    public string MiniLabel => PlanName;
    public string MiniRestLabel => IsRestTimerActive ? $"Recupero{(IsSuspended ? " in pausa" : "")} · {RestTimerText}" : "Nessun recupero attivo";
    public string RecoveryError { get; private set; } = "";
    public async Task RecoverAsync()
    {
        if (SuspendHandler != null) await SuspendHandler();
        End();
        SuspendHandler = null;
        ForegroundHandler = null;
        RecoveryError = "";
        try
        {
            var draft = (await db.GetWorkoutsAsync(pb.CurrentUser?.Id ?? "")).FirstOrDefault(w => w.IsDraft);
            if (draft == null) return;
            Start(draft.Name);
            UpdateElapsed(TimeSpan.FromSeconds(WorkoutCompletion.ElapsedSeconds(draft)).ToString(@"hh\:mm\:ss"));
            UpdateRestTimer(draft.DraftRestSeconds > 0, $"{draft.DraftRestSeconds / 60:00}:{draft.DraftRestSeconds % 60:00}");
            Suspend();
            Minimize();
        }
        catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"[Recovery] {ex.GetType().Name}"); RecoveryError = "Impossibile recuperare l’allenamento. Riprova dalla sezione Allenamento."; }
    }
    [RelayCommand]
    private async Task ResumeAsync()
    {
        if (!IsActive) return;
        await Shell.Current.GoToAsync("activeWorkout", new Dictionary<string, object> { ["mode"] = "resume" });
    }
    private void NotifyState()
    {
        OnPropertyChanged(nameof(ShowMiniBar));
        OnPropertyChanged(nameof(MiniLabel));
        OnPropertyChanged(nameof(MiniRestLabel));
        OnPropertyChanged(nameof(RestTimerText));
        OnPropertyChanged(nameof(IsRestTimerActive));
        StateChanged?.Invoke();
    }
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
        IsSuspended = false;
        IsMinimized = false;
        PlanName = planName;
        NotifyState();
    }

    public void Minimize()
    {
        IsMinimized = true;
        NotifyState();
    }

    public void Restore()
    {
        IsMinimized = false;
        IsSuspended = false;
        NotifyState();
    }

    public void Suspend()
    {
        IsSuspended = true;
        NotifyState();
    }

    public void UpdateElapsed(string elapsed)
    {
        ElapsedTime = elapsed;
        NotifyState();
    }

    public void UpdateRestTimer(bool active, string text)
    {
        IsRestTimerActive = active;
        RestTimerText = text;
        NotifyState();
    }

    public void End()
    {
        IsActive = false;
        IsSuspended = false;
        IsMinimized = false;
        PlanName = string.Empty;
        ElapsedTime = "00:00:00";
        RestTimerText = string.Empty;
        IsRestTimerActive = false;
        NotifyState();
    }
}
