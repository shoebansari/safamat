namespace Matrimonial.AdminApi.Common;

public static class DateTimeHelper
{
    public static DateTime ToUtcDate(DateTime date) =>
        DateTime.SpecifyKind(date.Date, DateTimeKind.Utc);

    public static DateTime? ToUtcDate(DateTime? date) =>
        date.HasValue ? ToUtcDate(date.Value) : null;
}
