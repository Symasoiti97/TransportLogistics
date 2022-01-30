using System.Globalization;
using Shared.Localization.Abstractions;

namespace Shared.Localization;

public class LocalizationProvider : ILocalizationProvider
{
    private CultureInfo _originalCulture;

    public void ResetCurrentCulture()
    {
        CultureInfo.CurrentCulture = _originalCulture;
        CultureInfo.CurrentUICulture = _originalCulture;
    }

    public CultureInfo GetCurrentUiCulture()
    {
        return CultureInfo.CurrentUICulture;
    }

    public void SetUICulture(CultureInfo cultureInfo)
    {
        CultureInfo.CurrentUICulture = cultureInfo ?? throw new ArgumentNullException(nameof(cultureInfo));
    }

    public string GetLanguage()
    {
        return CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
    }
}

