using System;
using System.IO;
using T4VPN.Common.Extensions;
using T4VPN.Common.Logging;
using T4VPN.Modals;

namespace T4VPN.About
{
    public class LicenseModalViewModel : BaseModalViewModel
    {
        private readonly ILogger _logger;
        private const string LicenseFile = "COPYING.md";

        public LicenseModalViewModel(ILogger logger)
        {
            _logger = logger;
        }

        public string License { get; set; }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            LoadLicense();
        }

        private void LoadLicense()
        {
            try
            {
                License = File.ReadAllText(LicenseFile);
            }
            catch (Exception e) when (e.IsFileAccessException())
            {
                _logger.Error(e);
            }
        }
    }
}
