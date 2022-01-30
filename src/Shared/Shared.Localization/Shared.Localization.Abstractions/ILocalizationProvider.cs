using System.Globalization;

namespace Shared.Localization.Abstractions;

public interface ILocalizationProvider
{
    CultureInfo GetCurrentUiCulture();
    string GetLanguage();
}
