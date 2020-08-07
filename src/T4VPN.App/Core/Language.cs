using System.Globalization;
using System.Linq;
using System.Threading;
using T4VPN.Common.Cli;
using T4VPN.Core.Auth;
using T4VPN.Core.Settings;
using T4VPN.Resources;

namespace T4VPN.Core
{
    public class Language : ILoggedInAware
    {
        private readonly IAppSettings _appSettings;
        private readonly ILanguageProvider _languageProvider;
        private readonly string _defaultLocale;
        private string _startupLanguage;

        public Language(IAppSettings appSettings, ILanguageProvider languageProvider, string defaultLocale)
        {
            _defaultLocale = defaultLocale;
            _languageProvider = languageProvider;
            _appSettings = appSettings;
        }

        public void Initialize(string[] args)
        {
            var lang = GetCommandLineLanguage(args);
            if (_languageProvider.GetAll().Contains(lang))
            {
                _startupLanguage = lang;
            }
            else
            {
                lang = GetStartupLanguage();
            }

            TranslationSource.Instance.CurrentCulture = new CultureInfo(lang);
        }

        public void OnUserLoggedIn()
        {
            if (!string.IsNullOrEmpty(_startupLanguage))
            {
                _appSettings.Language = _startupLanguage;
            }
            else if (string.IsNullOrEmpty(_appSettings.Language))
            {
                _appSettings.Language = GetCurrentLanguage();
            }
        }

        private string GetCommandLineLanguage(string[] args)
        {
            var option = new CommandLineOption("lang", args);
            return option.Params().FirstOrDefault();
        }

        private string GetStartupLanguage()
        {
            return string.IsNullOrEmpty(_appSettings.Language) ? GetCurrentLanguage() : _appSettings.Language;
        }

        private string GetCurrentLanguage()
        {
            var osLanguage = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
            return _languageProvider.GetAll().Contains(osLanguage) ? osLanguage : _defaultLocale;
        }
    }
}
