using System.Globalization;
using Shared.Localization.Abstractions;

namespace Shared.Localization;

public class LocalizationProvider : ILocalizationProvider
{
    private CultureInfo _originalCulture;

    public void SetCurrentCulture(CultureInfo culture)
    {
        _originalCulture = CultureInfo.CurrentCulture;
        var cultureInfo = culture ?? CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = cultureInfo;
        CultureInfo.CurrentUICulture = cultureInfo;
    }

    public void ResetCurrentCulture()
    {
        CultureInfo.CurrentCulture = _originalCulture;
        CultureInfo.CurrentUICulture = _originalCulture;
    }

    public CultureInfo GetCurrentCulture()
    {
        return CultureInfo.CurrentCulture;
    }

    public CultureInfo GetCurrentUICulture()
    {
        return CultureInfo.CurrentUICulture;
    }
        
    public void SetCulture(CultureInfo cultureInfo)
    {
        CultureInfo.CurrentCulture = cultureInfo ?? throw new ArgumentNullException(nameof(cultureInfo));
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

