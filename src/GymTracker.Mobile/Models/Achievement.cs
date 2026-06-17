using SQLite;

namespace GymTracker.Mobile.Models;

[Table("achievements")]
public class AchievementState
{
    [PrimaryKey]
    public string Id { get; set; } = string.Empty;
    public int Progress { get; set; }
    public int MaxProgress { get; set; } = 1;
    public bool IsUnlocked { get; set; }
    public string UnlockedAt { get; set; } = string.Empty;
    public bool Notified { get; set; }
}

public class AchievementDefinition
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Icon { get; set; } = "🏆";
    public int MaxProgress { get; set; } = 1;
}

public static class AchievementsCatalog
{
    public static List<AchievementDefinition> All { get; } = new()
    {
        // 📅 Costanza e Abitudine
        new() { Id = "first_workout", Name = "Primo Passo", Description = "Completa il tuo primo allenamento", Category = "Costanza", Icon = "👣", MaxProgress = 1 },
        new() { Id = "holy_week", Name = "Settimana Santa", Description = "Allenati per 7 giorni consecutivi", Category = "Costanza", Icon = "📅", MaxProgress = 7 },
        new() { Id = "habit", Name = "Abitudinario", Description = "Completa 10 allenamenti in un mese", Category = "Costanza", Icon = "🔄", MaxProgress = 10 },
        new() { Id = "weekend_warrior", Name = "Guerriero del Weekend", Description = "Allenati sia sabato che domenica", Category = "Costanza", Icon = "⚔️", MaxProgress = 2 },
        new() { Id = "no_excuses", Name = "Niente Scuse", Description = "Allenati in un giorno festivo", Category = "Costanza", Icon = "🚫", MaxProgress = 1 },
        new() { Id = "unstoppable", Name = "Inarrestabile", Description = "Raggiungi una serie di 30 giorni", Category = "Costanza", Icon = "🔥", MaxProgress = 30 },
        new() { Id = "member", Name = "Abbonato Fisso", Description = "Completa 100 allenamenti totali", Category = "Costanza", Icon = "💯", MaxProgress = 100 },
        new() { Id = "comeback", Name = "Ritorno al Futuro", Description = "Riprendi ad allenarti dopo 2 settimane di pausa", Category = "Costanza", Icon = "🔙", MaxProgress = 1 },
        new() { Id = "metronome", Name = "Metronomo", Description = "Allenati alla stessa ora per 5 volte di fila", Category = "Costanza", Icon = "⏱️", MaxProgress = 5 },
        new() { Id = "anniversary", Name = "Anniversario", Description = "Un anno dall'iscrizione all'app", Category = "Costanza", Icon = "🎂", MaxProgress = 1 },

        // 🏋️ Forza e Performance
        new() { Id = "max_weight", Name = "Peso Massimo", Description = "Solleva 1.000 kg in una sessione", Category = "Forza", Icon = "🏋️", MaxProgress = 1000 },
        new() { Id = "club_100", Name = "Club dei 100", Description = "Esegui una serie con 100 kg", Category = "Forza", Icon = "💪", MaxProgress = 1 },
        new() { Id = "hercules", Name = "Ercole", Description = "Supera il tuo massimale (PR) 5 volte", Category = "Forza", Icon = "🦁", MaxProgress = 5 },
        new() { Id = "iron_marathon", Name = "Maratoneta di Ghisa", Description = "Solleva 10.000 kg in una settimana", Category = "Forza", Icon = "🏃", MaxProgress = 10000 },
        new() { Id = "steel_plank", Name = "Plank d'Acciaio", Description = "Registra un plank di oltre 3 minuti", Category = "Forza", Icon = "🪨", MaxProgress = 1 },
        new() { Id = "marble_legs", Name = "Gambe di Marmo", Description = "Completa 50 squat in una sessione", Category = "Forza", Icon = "🦵", MaxProgress = 50 },
        new() { Id = "bench_king", Name = "Pressione Alta", Description = "Completa 10 sessioni di panca piana", Category = "Forza", Icon = "🏋️", MaxProgress = 10 },
        new() { Id = "powerlifter", Name = "Powerlifter in Erba", Description = "Registra un PR per Squat, Panca e Stacco", Category = "Forza", Icon = "🏆", MaxProgress = 3 },
        new() { Id = "ton_week", Name = "Volume Folle", Description = "Solleva 50.000 kg in un mese", Category = "Forza", Icon = "📊", MaxProgress = 50000 },

        // ⏰ Orari
        new() { Id = "early_bird", Name = "Il Mattino ha l'Oro", Description = "Allenati prima delle 7:00", Category = "Orari", Icon = "🌅", MaxProgress = 1 },
        new() { Id = "night_owl", Name = "Creatura della Notte", Description = "Allenati dopo le 22:00", Category = "Orari", Icon = "🦉", MaxProgress = 1 },
        new() { Id = "lunch_break", Name = "Pausa Pranzo Dinamica", Description = "Allenati tra le 12:00 e le 14:00", Category = "Orari", Icon = "🥗", MaxProgress = 1 },

        // 🧬 Varietà
        new() { Id = "explorer", Name = "Esploratore", Description = "Prova 5 esercizi diversi in un allenamento", Category = "Varietà", Icon = "🧭", MaxProgress = 5 },
        new() { Id = "all_rounder", Name = "Tuttofare", Description = "Allena ogni gruppo muscolare", Category = "Varietà", Icon = "🎯", MaxProgress = 8 },
        new() { Id = "pioneer", Name = "Pioniere", Description = "Prova una nuova scheda d'allenamento", Category = "Varietà", Icon = "🚀", MaxProgress = 1 },
        new() { Id = "cardio_heart", Name = "Cuore d'Atleta", Description = "Completa 30 minuti di cardio", Category = "Varietà", Icon = "❤️", MaxProgress = 30 },
        new() { Id = "full_body", Name = "Full Body Master", Description = "Allena tutto il corpo in una sessione", Category = "Varietà", Icon = "🌟", MaxProgress = 1 },
        new() { Id = "big_three", Name = "Amante dei Classici", Description = "Esegui Squat, Panca e Stacco lo stesso giorno", Category = "Varietà", Icon = "🔱", MaxProgress = 1 },
        new() { Id = "encyclopedia", Name = "Enciclopedia", Description = "Esegui 50 esercizi diversi", Category = "Varietà", Icon = "📚", MaxProgress = 50 },

        // 🎈 Social e Foto
        new() { Id = "narcissus", Name = "Narciso", Description = "Aggiungi una foto al tuo allenamento", Category = "Social", Icon = "📸", MaxProgress = 1 },
        new() { Id = "photographer", Name = "Fotografo", Description = "Aggiungi foto a 10 allenamenti", Category = "Social", Icon = "📷", MaxProgress = 10 },
        new() { Id = "social_butterfly", Name = "Socialite", Description = "Ricevi 50 like totali", Category = "Social", Icon = "🦋", MaxProgress = 50 },
        new() { Id = "popular", Name = "Popolare", Description = "Ottieni 10 like su un singolo allenamento", Category = "Social", Icon = "⭐", MaxProgress = 10 },
        new() { Id = "networker", Name = "Networker", Description = "Segui 5 amici", Category = "Social", Icon = "🤝", MaxProgress = 5 },

        // 🏆 Sfide Elite
        new() { Id = "gym_king", Name = "Il Re della Palestra", Description = "Allenati 5 giorni a settimana per 12 settimane", Category = "Elite", Icon = "👑", MaxProgress = 60 },
        new() { Id = "ninja", Name = "Ninja", Description = "Completa un allenamento senza modificare i recuperi", Category = "Elite", Icon = "🥷", MaxProgress = 1 },
        new() { Id = "veteran", Name = "Veterano", Description = "Completa 500 allenamenti totali", Category = "Elite", Icon = "🎖️", MaxProgress = 500 },
        new() { Id = "milestone", Name = "Pietra Miliare", Description = "Raggiungi 100 ore totali di allenamento", Category = "Elite", Icon = "⏳", MaxProgress = 100 },
        new() { Id = "fitness_god", Name = "Divinità del Fitness", Description = "Sblocca tutti gli altri achievement", Category = "Elite", Icon = "⚡", MaxProgress = 1 },
    };
}
