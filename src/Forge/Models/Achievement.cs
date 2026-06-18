using SQLite;

namespace Forge.Models;

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
        new() { Id = "first_workout", Name = "Primo Passo", Description = "Completa il tuo primo allenamento", Category = "Costanza", Icon = "achievements/primopasso.png", MaxProgress = 1 },
        new() { Id = "holy_week", Name = "Settimana Santa", Description = "Allenati per 7 giorni consecutivi", Category = "Costanza", Icon = "achievements/holy_week.png", MaxProgress = 7 },
        new() { Id = "habit", Name = "Abitudinario", Description = "Completa 10 allenamenti in un mese", Category = "Costanza", Icon = "achievements/habit.png", MaxProgress = 10 },
        new() { Id = "weekend_warrior", Name = "Guerriero del Weekend", Description = "Allenati sia sabato che domenica", Category = "Costanza", Icon = "achievements/weekend_warrior.png", MaxProgress = 2 },
        new() { Id = "no_excuses", Name = "Niente Scuse", Description = "Allenati in un giorno festivo", Category = "Costanza", Icon = "achievements/no_exxcuses.png", MaxProgress = 1 },
        new() { Id = "unstoppable", Name = "Inarrestabile", Description = "Raggiungi una serie di 30 giorni", Category = "Costanza", Icon = "achievements/unstoppable.png", MaxProgress = 30 },
        new() { Id = "member", Name = "Abbonato Fisso", Description = "Completa 100 allenamenti totali", Category = "Costanza", Icon = "achievements/member.png", MaxProgress = 100 },
        new() { Id = "comeback", Name = "Ritorno al Futuro", Description = "Riprendi ad allenarti dopo 2 settimane di pausa", Category = "Costanza", Icon = "achievements/comeback.png", MaxProgress = 1 },
        new() { Id = "metronome", Name = "Metronomo", Description = "Allenati alla stessa ora per 5 volte di fila", Category = "Costanza", Icon = "achievements/metrometer.png", MaxProgress = 5 },
        new() { Id = "anniversary", Name = "Anniversario", Description = "Un anno dall'iscrizione all'app", Category = "Costanza", Icon = "achievements/anniversary.png", MaxProgress = 1 },

        new() { Id = "max_weight", Name = "Peso Massimo", Description = "Solleva 1.000 kg in una sessione", Category = "Forza", Icon = "achievements/max_weight.png", MaxProgress = 1000 },
        new() { Id = "club_100", Name = "Club dei 100", Description = "Esegui una serie con 100 kg", Category = "Forza", Icon = "achievements/club_100.png", MaxProgress = 1 },
        new() { Id = "hercules", Name = "Ercole", Description = "Supera il tuo massimale (PR) 5 volte", Category = "Forza", Icon = "achievements/hercules.png", MaxProgress = 5 },
        new() { Id = "iron_marathon", Name = "Maratoneta di Ghisa", Description = "Solleva 10.000 kg in una settimana", Category = "Forza", Icon = "achievements/ironmaraton.png", MaxProgress = 10000 },
        new() { Id = "steel_plank", Name = "Plank d'Acciaio", Description = "Registra un plank di oltre 3 minuti", Category = "Forza", Icon = "achievements/steel_plank.png", MaxProgress = 1 },
        new() { Id = "marble_legs", Name = "Gambe di Marmo", Description = "Completa 50 squat in una sessione", Category = "Forza", Icon = "achievements/marbleleg.png", MaxProgress = 50 },
        new() { Id = "bench_king", Name = "Pressione Alta", Description = "Completa 10 sessioni di panca piana", Category = "Forza", Icon = "achievements/benchking.png", MaxProgress = 10 },
        new() { Id = "powerlifter", Name = "Powerlifter in Erba", Description = "Registra un PR per Squat, Panca e Stacco", Category = "Forza", Icon = "achievements/powerlifter.png", MaxProgress = 3 },
        new() { Id = "ton_week", Name = "Volume Folle", Description = "Solleva 50.000 kg in un mese", Category = "Forza", Icon = "achievements/tonweek.png", MaxProgress = 50000 },

        new() { Id = "early_bird", Name = "Il Mattino ha l'Oro", Description = "Allenati prima delle 7:00", Category = "Orari", Icon = "achievements/earlyboard.png", MaxProgress = 1 },
        new() { Id = "night_owl", Name = "Creatura della Notte", Description = "Allenati dopo le 22:00", Category = "Orari", Icon = "achievements/nightowl.png", MaxProgress = 1 },
        new() { Id = "punctual", Name = "Puntuale", Description = "Inizia l'allenamento all'orario programmato", Category = "Orari", Icon = "achievements/punctual.png", MaxProgress = 1 },
        new() { Id = "hour_grinder", Name = "Hour Grinder", Description = "Completa 10 ore totali di allenamento", Category = "Orari", Icon = "achievements/hourgrinder.png", MaxProgress = 10 },

        new() { Id = "explorer", Name = "Esploratore", Description = "Prova 5 esercizi diversi in un allenamento", Category = "Varietà", Icon = "achievements/explorer.png", MaxProgress = 5 },
        new() { Id = "all_rounder", Name = "Tuttofare", Description = "Allena ogni gruppo muscolare", Category = "Varietà", Icon = "achievements/jackofall.png", MaxProgress = 8 },
        new() { Id = "specialist", Name = "Specialista", Description = "Completa 10 allenamenti per lo stesso gruppo muscolare", Category = "Varietà", Icon = "achievements/specialist.png", MaxProgress = 10 },
        new() { Id = "full_body", Name = "Full Body Master", Description = "Allena tutto il corpo in una sessione", Category = "Varietà", Icon = "achievements/bodyweightmaster.png", MaxProgress = 1 },
        new() { Id = "encyclopedia", Name = "Enciclopedia", Description = "Esegui 50 esercizi diversi", Category = "Varietà", Icon = "achievements/explorer.png", MaxProgress = 50 },

        new() { Id = "narcissus", Name = "Narciso", Description = "Aggiungi una foto al tuo allenamento", Category = "Social", Icon = "achievements/member.png", MaxProgress = 1 },
        new() { Id = "photographer", Name = "Fotografo", Description = "Aggiungi foto a 10 allenamenti", Category = "Social", Icon = "achievements/powerlifter.png", MaxProgress = 10 },
        new() { Id = "social_butterfly", Name = "Influencer", Description = "Ricevi 50 like totali", Category = "Social", Icon = "achievements/influencer.png", MaxProgress = 50 },
        new() { Id = "networker", Name = "Networker", Description = "Segui 5 amici", Category = "Social", Icon = "achievements/gymbuddy.png", MaxProgress = 5 },
        new() { Id = "gym_buddy", Name = "Gym Buddy", Description = "Allenati con un amico lo stesso giorno", Category = "Social", Icon = "achievements/gymbuddy.png", MaxProgress = 1 },
        new() { Id = "lone_wolf", Name = "Lupo Solitario", Description = "Completa 20 allenamenti senza seguire nessuno", Category = "Social", Icon = "achievements/lonewolf.png", MaxProgress = 20 },
        new() { Id = "mentor", Name = "Mentore", Description = "Un amico inizia ad allenarsi dopo averti seguito", Category = "Social", Icon = "achievements/mentor.png", MaxProgress = 1 },

        new() { Id = "champion", Name = "Campione", Description = "Raggiungi la vetta della classifica volume mensile", Category = "Elite", Icon = "achievements/champion.png", MaxProgress = 1 },
        new() { Id = "serial_winner", Name = "Serial Winner", Description = "Vinci la classifica per 3 mesi consecutivi", Category = "Elite", Icon = "achievements/serialwinner.png", MaxProgress = 3 },
        new() { Id = "challenger", Name = "Sfidante", Description = "Supera il volume di un amico per 5 volte", Category = "Elite", Icon = "achievements/challenger.png", MaxProgress = 5 },
        new() { Id = "underdog", Name = "Underdog", Description = "Batti un avversario con più allenamenti di te", Category = "Elite", Icon = "achievements/underdog.png", MaxProgress = 1 },
        new() { Id = "founder", Name = "Fondatore", Description = "Registrati entro la prima settimana dal lancio", Category = "Elite", Icon = "achievements/founder.png", MaxProgress = 1 },
        new() { Id = "beta_tester", Name = "Beta Tester", Description = "Segnala un bug o dai un feedback", Category = "Elite", Icon = "achievements/betatester.png", MaxProgress = 1 },
        new() { Id = "legend", Name = "Leggenda", Description = "Sblocca tutti gli altri achievement", Category = "Elite", Icon = "achievements/legend.png", MaxProgress = 1 },

        new() { Id = "veteran", Name = "Veterano", Description = "Completa 500 allenamenti totali", Category = "Elite", Icon = "achievements/legend.png", MaxProgress = 500 },
        new() { Id = "milestone", Name = "Pietra Miliare", Description = "Raggiungi 100 ore totali di allenamento", Category = "Elite", Icon = "achievements/hourgrinder.png", MaxProgress = 100 },
    };
}
