using System.Globalization;

namespace PersonalFinanceWeb.Client.Services;

/// <inheritdoc cref="ICurrencyFormatterService"/>
public sealed class CurrencyFormatterService(IProfileClientService profileService)
    : ICurrencyFormatterService
{
    // Hardcoded map of every currency offered in Profile.razor → the best-matching
    // CultureInfo name that produces the correct locale symbol + number format.
    private static readonly Dictionary<string, string> CurrencyCultureMap = new(StringComparer.OrdinalIgnoreCase)
    {
        { "USD", "en-US" },
        { "EUR", "fr-FR" },   // € with space-separated thousands (common European style)
        { "GBP", "en-GB" },
        { "IDR", "id-ID" },
        { "JPY", "ja-JP" },
        { "AUD", "en-AU" },
        { "CAD", "en-CA" },
        { "CHF", "de-CH" },
        { "CNY", "zh-CN" },
        { "INR", "hi-IN" },
        { "SGD", "en-SG" },
        { "MYR", "ms-MY" },
        { "THB", "th-TH" },
        { "KRW", "ko-KR" },
        { "HKD", "zh-HK" },
    };

    private bool _initialized;

    // ── Public state ────────────────────────────────────────────────────────────

    public bool IsInitialized => _initialized;
    public string CurrencyCode { get; private set; } = "USD";
    public string CurrencySymbol { get; private set; } = "$";
    public CultureInfo Culture { get; private set; } = CultureInfo.GetCultureInfo("en-US");

    public event Action? OnChange;

    // ── Initialization ──────────────────────────────────────────────────────────

    public async Task InitializeAsync()
    {
        if (_initialized) return;

        try
        {
            var profile = await profileService.GetProfileAsync();
            if (profile is not null && !string.IsNullOrWhiteSpace(profile.DefaultCurrency))
                ApplyCurrency(profile.DefaultCurrency);
        }
        catch
        {
            // Fall back to USD if the profile cannot be fetched (e.g. during SSR pre-render).
        }
        finally
        {
            _initialized = true;
            OnChange?.Invoke();
        }
    }

    // ── Formatting / Parsing ────────────────────────────────────────────────────

    public string Format(decimal amount)
        => amount.ToString("C", Culture);

    public decimal Parse(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return 0m;

        // Strip the currency symbol so that the locale number parser can handle the rest.
        var cleaned = input
            .Replace(CurrencySymbol, string.Empty, StringComparison.OrdinalIgnoreCase)
            .Trim();

        return decimal.TryParse(cleaned, NumberStyles.Number, Culture.NumberFormat, out var value)
            ? value
            : 0m;
    }

    // ── Helpers ─────────────────────────────────────────────────────────────────

    private void ApplyCurrency(string code)
    {
        CurrencyCode = code.ToUpperInvariant();

        var cultureName = CurrencyCultureMap.TryGetValue(code, out var name) ? name : "en-US";
        Culture = CultureInfo.GetCultureInfo(cultureName);

        // Derive the display symbol from the resolved culture's NumberFormat.
        CurrencySymbol = Culture.NumberFormat.CurrencySymbol;
    }
}
