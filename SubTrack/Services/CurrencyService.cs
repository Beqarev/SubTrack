namespace SubTrack.Services;

public static class CurrencyService
{
    public const string DefaultCurrencyCode = "USD";
    public const string GelCurrencyCode = "GEL";

    private const decimal UsdToGelRate = 2.70m;

    public static IReadOnlyList<string> SupportedCurrencyCodes { get; } = [DefaultCurrencyCode, GelCurrencyCode];

    public static bool IsSupported(string? currencyCode) =>
        SupportedCurrencyCodes.Contains(Normalize(currencyCode));

    public static string Normalize(string? currencyCode)
    {
        var normalized = string.IsNullOrWhiteSpace(currencyCode)
            ? DefaultCurrencyCode
            : currencyCode.Trim().ToUpperInvariant();

        return SupportedCurrencyCodes.Contains(normalized) ? normalized : DefaultCurrencyCode;
    }

    public static decimal Convert(decimal amount, string? fromCurrencyCode, string? toCurrencyCode)
    {
        var from = Normalize(fromCurrencyCode);
        var to = Normalize(toCurrencyCode);

        if (from == to)
        {
            return decimal.Round(amount, 2);
        }

        var amountInUsd = from == GelCurrencyCode ? amount / UsdToGelRate : amount;
        var converted = to == GelCurrencyCode ? amountInUsd * UsdToGelRate : amountInUsd;

        return decimal.Round(converted, 2);
    }

    public static string Format(decimal amount, string? currencyCode, int decimals = 2)
    {
        var normalized = Normalize(currencyCode);
        var format = decimals <= 0 ? "0" : "0." + new string('0', decimals);

        return normalized == DefaultCurrencyCode
            ? "$" + amount.ToString(format)
            : normalized + " " + amount.ToString(format);
    }
}
