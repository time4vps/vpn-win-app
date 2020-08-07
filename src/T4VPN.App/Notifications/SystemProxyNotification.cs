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

using System.Threading.Tasks;
using T4VPN.Common.OS.Registry;
using T4VPN.Common.Vpn;
using T4VPN.Core.Modals;
using T4VPN.Core.Vpn;
using T4VPN.Resources;

namespace T4VPN.Notifications
{
    internal class SystemProxyNotification : IVpnStateAware
    {
        private bool _modalShown;
        private readonly ISystemProxy _proxy;
        private readonly IDialogs _dialogs;

        public SystemProxyNotification(ISystemProxy proxy, IDialogs dialogs)
        {
            _proxy = proxy;
            _dialogs = dialogs;
        }

        public Task OnVpnStateChanged(VpnStateChangedEventArgs e)
        {
            if (e.State.Status == VpnStatus.Connected && _proxy.Enabled() && !_modalShown)
            {
                _modalShown = true;
                _dialogs.ShowWarning(StringResources.Get("Dialogs_Proxy_msg_ProxyDetected"));
            }

            if (e.State.Status == VpnStatus.Disconnected)
            {
                _modalShown = false;
            }

            return Task.CompletedTask;
        }
    }
}
