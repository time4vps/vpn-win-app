﻿using System.Collections.Generic;
using T4VPN.Core.Settings;

namespace T4VPN.Settings.ReconnectNotification
{
    public class SettingsBuilder
    {
        private List<Setting> _settings;
        private readonly IAppSettings _appSettings;

        public SettingsBuilder(IAppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public List<Setting> Build()
        {
            _settings = new List<Setting>
            {
                new SingleSetting(nameof(IAppSettings.KillSwitch), null, _appSettings),
                new SingleSetting(nameof(IAppSettings.OvpnProtocol), null, _appSettings),
                new CustomDnsSetting(nameof(IAppSettings.CustomDnsEnabled), null, _appSettings)
            };

            var st = new CompoundSetting(nameof(IAppSettings.SplitTunnelingEnabled), null, _appSettings);
            st.Add(new SplitTunnelModeSetting(nameof(IAppSettings.SplitTunnelMode), st, _appSettings));

            _settings.Add(st);

            AddNetShieldSettings();

            return _settings;
        }

        private void AddNetShieldSettings()
        {
            var ns = new CompoundSetting(nameof(IAppSettings.NetShieldEnabled), null, _appSettings);
            ns.Add(new SingleSetting(nameof(IAppSettings.NetShieldMode), ns, _appSettings));

            _settings.Add(ns);
        }
    }
}
