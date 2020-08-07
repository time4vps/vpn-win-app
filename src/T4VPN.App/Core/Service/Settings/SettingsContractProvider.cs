﻿/*
 * Copyright (c) 2020 Time4VPS
 *
 * This file is part of T4VPN.
 *
 * T4VPN is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * T4VPN is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with T4VPN.  If not, see <https://www.gnu.org/licenses/>.
 */

using T4VPN.Common;
using T4VPN.Core.Settings;
using T4VPN.Service.Contract.Settings;
using System.Linq;

namespace T4VPN.Core.Service.Settings
{
    public class SettingsContractProvider
    {
        private readonly IAppSettings _appSettings;

        public SettingsContractProvider(IAppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public SettingsContract GetSettingsContract()
        {
            return new SettingsContract
            {
                KillSwitch = new KillSwitchSettingsContract
                {
                    Enabled = _appSettings.KillSwitch
                },
                SplitTunnel = new SplitTunnelSettingsContract
                {
                    Mode = _appSettings.SplitTunnelingEnabled ? _appSettings.SplitTunnelMode : SplitTunnelMode.Disabled,
                    AppPaths = _appSettings.GetSplitTunnelApps(),
                    Ips = GetSplitTunnelIps()
                },
                Ipv6LeakProtection = _appSettings.Ipv6LeakProtection,
            };
        }

        private string[] GetSplitTunnelIps()
        {
            switch (_appSettings.SplitTunnelMode)
            {
                case SplitTunnelMode.Permit:
                    return _appSettings.SplitTunnelIncludeIps.Where(i => i.Enabled).Select(i => i.Ip).ToArray();
                case SplitTunnelMode.Block:
                    return _appSettings.SplitTunnelExcludeIps.Where(i => i.Enabled).Select(i => i.Ip).ToArray();
                default:
                    return new string[] { };
            }
        }
    }
}
