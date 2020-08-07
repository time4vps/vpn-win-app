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

using T4VPN.Common.Vpn;
using T4VPN.Core.Modals;
using T4VPN.Core.Vpn;
using T4VPN.Resources;
using System.Threading.Tasks;
using System.Windows;

namespace T4VPN.Core
{
    internal class AppExitHandler : IVpnStateAware
    {
        private readonly IDialogs _dialogs;
        private VpnStatus _vpnStatus;

        public bool PendingExit { get; private set; }

        public AppExitHandler(IDialogs dialogs)
        {
            _dialogs = dialogs;
        }

        public void RequestExit()
        {
            if (_vpnStatus != VpnStatus.Disconnected &&
                _vpnStatus != VpnStatus.Disconnecting)
            {
                var result = _dialogs.ShowQuestion(StringResources.Get("App_msg_ExitConnectedConfirm"));
                if (result == false)
                    return;
            }

            PendingExit = true;

            Application.Current.Shutdown();
        }

        public Task OnVpnStateChanged(VpnStateChangedEventArgs e)
        {
            _vpnStatus = e.State.Status;
            return Task.CompletedTask;
        }
    }
}
