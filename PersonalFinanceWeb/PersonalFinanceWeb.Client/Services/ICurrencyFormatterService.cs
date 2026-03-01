using System.Globalization;

namespace PersonalFinanceWeb.Client.Services;

/// <summary>
/// Provides locale-aware formatting and parsing of monetary values based on the
/// authenticated user's <c>DefaultCurrency</c> preference stored in their profile.
/// Initialize once (e.g. in <c>MainLayout</c>) by calling <see cref="InitializeAsync"/>;
/// subsequently all pages share the same scoped instance with consistent formatting.
/// </summary>
public interface ICurrencyFormatterService
{
    /// <summary>Whether <see cref="InitializeAsync"/> has completed successfully.</summary>
    bool IsInitialized { get; }

    /// <summary>ISO 4217 currency code, e.g. "USD", "EUR", "IDR".</summary>
    string CurrencyCode { get; }

    /// <summary>Locale-specific currency symbol, e.g. "$", "€", "Rp".</summary>
    string CurrencySymbol { get; }

    /// <summary>The resolved <see cref="CultureInfo"/> used for all formatting / parsing.</summary>
    CultureInfo Culture { get; }

    /// <summary>
    /// Format <paramref name="amount"/> as a currency string using the user's locale
    /// (symbol + locale grouping/decimal separators), e.g. "$1,234.56" or "Rp1.234".
    /// </summary>
    string Format(decimal amount);

    /// <summary>
    /// Parse a user-entered string (which may include the currency symbol) back to a
    /// <see cref="decimal"/>.  Returns 0 if parsing fails.
    /// </summary>
    decimal Parse(string input);

    /// <summary>
    /// Load the user profile, resolve the matching <see cref="CultureInfo"/> for the
    /// stored <c>DefaultCurrency</c> code, and cache the result.  Safe to call multiple
    /// times (subsequent calls are no-ops).
    /// </summary>
    Task InitializeAsync();

    /// <summary>Raised after a successful <see cref="InitializeAsync"/> call.</summary>
    event Action? OnChange;
}
