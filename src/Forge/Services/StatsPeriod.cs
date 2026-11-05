namespace Forge.Services;

public static class StatsPeriod
{
    public static (DateTime Start, DateTime EndExclusive) Range(string filter, DateTime today, DateTime? earliest = null, DateTime? selectedMonth = null)
    {
        if (filter == "month")
        {
            var month = selectedMonth ?? today;
            var first = new DateTime(month.Year, month.Month, 1);
            return (first, first.AddMonths(1));
        }
        return (Start(filter, today, earliest), today.Date.AddDays(1));
    }

    public static DateTime Start(string filter, DateTime today, DateTime? earliest = null) => filter switch
    {
        "week" => today.Date.AddDays(-((int)today.DayOfWeek + 6) % 7),
        "month" => new(today.Year, today.Month, 1),
        "3months" => new DateTime(today.Year, today.Month, 1).AddMonths(-2),
        "year" => new DateTime(today.Year, today.Month, 1).AddMonths(-11),
        _ => earliest?.Date ?? new DateTime(today.Year, today.Month, 1)
    };
}
