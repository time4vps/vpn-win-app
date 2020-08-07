/*
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
using T4VPN.Common.Vpn;
using T4VPN.NetworkFilter;
using T4VPN.Service.Firewall;
using T4VPN.Service.Settings;
using T4VPN.Service.Vpn;
using T4VPN.Vpn.Common;

namespace T4VPN.Service.SplitTunneling
{
    internal class SplitTunnel : IVpnStateAware
    {
        private bool _reverseEnabled;
        private bool _enabled;

        private readonly IServiceSettings _serviceSettings;
        private readonly IDriver _calloutDriver;
        private readonly IncludeModeApps _reverseSplitTunnelApps;
        private readonly ISplitTunnelClient _splitTunnelClient;
        private readonly IFilterCollection _appFilter;
        private readonly IFilterCollection _permittedRemoteAddress;

        public SplitTunnel(
            IServiceSettings serviceSettings,
            IDriver calloutDriver,
            ISplitTunnelClient splitTunnelClient,
            IncludeModeApps reverseSplitTunnelApps,
            IFilterCollection appFilter,
            IFilterCollection permittedRemoteAddress)
        {
            _permittedRemoteAddress = permittedRemoteAddress;
            _appFilter = appFilter;
            _splitTunnelClient = splitTunnelClient;
            _reverseSplitTunnelApps = reverseSplitTunnelApps;
            _calloutDriver = calloutDriver;
            _serviceSettings = serviceSettings;
        }

        internal SplitTunnel(
            bool enabled,
            bool reverseEnabled,
            IServiceSettings serviceSettings,
            IDriver calloutDriver,
            ISplitTunnelClient splitTunnelClient,
            IncludeModeApps reverseSplitTunnelApps,
            IFilterCollection appFilter,
            IFilterCollection permittedRemoteAddress) :
            this(serviceSettings,
                calloutDriver,
                splitTunnelClient,
                reverseSplitTunnelApps,
                appFilter,
                permittedRemoteAddress)
        {
            _enabled = enabled;
            _reverseEnabled = reverseEnabled;
        }

        public void OnVpnConnecting(VpnState state)
        {
            DisableReversed();
            Disable();
            _appFilter.RemoveAll();
            _permittedRemoteAddress.RemoveAll();

            if (_serviceSettings.SplitTunnelSettings.Mode == SplitTunnelMode.Permit)
            {
                _appFilter.Add(_serviceSettings.SplitTunnelSettings.AppPaths, Action.SoftBlock);
            }
        }

        public void OnVpnConnected(VpnState state)
        {
            if (_serviceSettings.SplitTunnelSettings.Mode == SplitTunnelMode.Disabled)
                return;

            switch (_serviceSettings.SplitTunnelSettings.Mode)
            {
                case SplitTunnelMode.Block:
                    DisableReversed();
                    Enable();
                    break;
                case SplitTunnelMode.Permit:
                    Disable();
                    EnableReversed(state);
                    _appFilter.RemoveAll();
                    break;
            }

            _calloutDriver.Start();
        }

        public void OnVpnDisconnected(VpnState state)
        {
            if (state.Error == VpnError.None)
            {
                DisableSplitTunnel();
                _appFilter.RemoveAll();
            }
        }

        private void DisableSplitTunnel()
        {
            _calloutDriver.Stop();
            Disable();
            DisableReversed();
        }

        private void Enable()
        {
            _splitTunnelClient.EnableExcludeMode(
                _serviceSettings.SplitTunnelSettings.AppPaths,
                _serviceSettings.SplitTunnelSettings.Ips);

            if (_serviceSettings.SplitTunnelSettings.AppPaths.Length > 0)
                _appFilter.Add(_serviceSettings.SplitTunnelSettings.AppPaths, Action.SoftPermit);

            if (_serviceSettings.SplitTunnelSettings.Ips.Length > 0)
                _permittedRemoteAddress.Add(_serviceSettings.SplitTunnelSettings.Ips, Action.SoftPermit);

            _enabled = true;
        }

        private void Disable()
        {
            if (!_enabled)
                return;

            _splitTunnelClient.Disable();
            _appFilter.RemoveAll();
            _permittedRemoteAddress.RemoveAll();
            _enabled = false;
        }

        private void EnableReversed(VpnState state)
        {
            _splitTunnelClient.EnableIncludeMode(
                _reverseSplitTunnelApps.Value(),
                _serviceSettings.SplitTunnelSettings.Ips,
                state.LocalIp);

            _reverseEnabled = true;
        }

        private void DisableReversed()
        {
            if (!_reverseEnabled)
                return;

            _splitTunnelClient.Disable();
            _reverseEnabled = false;
        }
    }
}
