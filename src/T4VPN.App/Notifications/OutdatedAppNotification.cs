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
using T4VPN.Common.Threading;
using T4VPN.Common.Vpn;
using T4VPN.Core.Api.Contracts;
using T4VPN.Core.Auth;
using T4VPN.Core.Modals;
using T4VPN.Core.Service.Vpn;
using T4VPN.Core.Vpn;
using T4VPN.Login.Views;
using T4VPN.Modals;

namespace T4VPN.Notifications
{
    internal class OutdatedAppNotification : IVpnStateAware
    {
        private readonly IModals _modals;
        private readonly UserAuth _userAuth;

        private bool _notified;
        private readonly LoginWindow _loginWindow;
        private readonly IScheduler _scheduler;
        private readonly IVpnServiceManager _vpnServiceManager;

        private VpnStatus _vpnStatus;

        public OutdatedAppNotification(
            IModals modals,
            UserAuth userAuth,
            LoginWindow loginWindow,
            IScheduler scheduler,
            IVpnServiceManager vpnServiceManager)
        {
            _modals = modals;
            _userAuth = userAuth;
            _loginWindow = loginWindow;
            _scheduler = scheduler;
            _vpnServiceManager = vpnServiceManager;
        }

        public void OnAppOutdated(object sender, BaseResponse e)
        {
            if (_notified)
            {
                return;
            }

            _notified = true;
            _scheduler.Schedule(() =>
            {
                if (_vpnStatus != VpnStatus.Disconnected)
                {
                    _vpnServiceManager.Disconnect(VpnError.Unknown);
                }

                _userAuth.Logout();
                _loginWindow.Hide();
                _modals.Show<OutdatedAppModalViewModel>();
            });
        }

        public Task OnVpnStateChanged(VpnStateChangedEventArgs e)
        {
            _vpnStatus = e.State.Status;

            return Task.CompletedTask;
        }
    }
}
