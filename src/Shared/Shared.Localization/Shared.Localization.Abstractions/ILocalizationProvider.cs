using System.Globalization;

namespace Shared.Localization.Abstractions;

public interface ILocalizationProvider
{
    CultureInfo GetCurrentCulture();
    CultureInfo GetCurrentUICulture();
    void SetCulture(CultureInfo cultureInfo);
    void SetUICulture(CultureInfo cultureInfo);
    string GetLanguage();

    void SetCurrentCulture(CultureInfo culture);
    void ResetCurrentCulture();
}
