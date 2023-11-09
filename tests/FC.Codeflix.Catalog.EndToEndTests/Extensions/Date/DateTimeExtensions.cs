namespace FC.Codeflix.Catalog.EndToEndTests.Extensions.Date;
internal static class DateTimeExtensions
{
    public static DateTime TrimMillisseconds(this DateTime dateTime) =>
        new(dateTime.Year,
            dateTime.Month,
            dateTime.Day,
            dateTime.Hour,
            dateTime.Minute,
            dateTime.Second,
            0,
            dateTime.Kind);
}
