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

using System;
using System.Dynamic;
using System.Threading;
using System.Threading.Tasks;
using T4VPN.Common.Vpn;
using T4VPN.Core.Auth;
using T4VPN.Core.Modals;
using T4VPN.Core.Settings;
using T4VPN.Core.Vpn;
using T4VPN.Modals;

namespace T4VPN.Vpn
{
    public class DisconnectError : IVpnStateAware, ILogoutAware, ILoggedInAware
    {
        private readonly IModals _modals;
        private readonly IAppSettings _appSettings;

        private bool _connected;
        private bool _loggedIn;


        public DisconnectError(IModals modals, IAppSettings appSettings)
        {
            _modals = modals;
            _appSettings = appSettings;
        }

        public Task OnVpnStateChanged(VpnStateChangedEventArgs e)
        {
            var status = e.State.Status;

            if (ModalShouldBeShown(e))
            {
                Post(() => ShowModal(e));
            }
            else
            {
                if (status == VpnStatus.Connecting ||
                    status == VpnStatus.Connected ||
                    (status == VpnStatus.Disconnecting ||
                     status == VpnStatus.Disconnected) &&
                    e.Error == VpnError.None)
                {
                    Post(CloseModal);
                }

                _connected = status == VpnStatus.Connected;
            }

            return Task.CompletedTask;
        }

        private bool ModalShouldBeShown(VpnStateChangedEventArgs e)
        {
            return _loggedIn && e.Error != VpnError.NoneKeepEnabledKillSwitch && (Reconnecting(e) || e.UnexpectedDisconnect);
        }

        private bool Reconnecting(VpnStateChangedEventArgs e)
        {
            return e.State.Status == VpnStatus.Reconnecting &&
                   e.NetworkBlocked &&
                   _connected &&
                   _appSettings.KillSwitch;
        }

        private void ShowModal(VpnStateChangedEventArgs e)
        {
            dynamic options = new ExpandoObject();
            options.NetworkBlocked = e.NetworkBlocked;
            options.Error = e.Error;

            _modals.Show<DisconnectErrorModalViewModel>(options);
        }

        private void CloseModal()
        {
            _modals.Close<DisconnectErrorModalViewModel>();
        }

        private void Post(Action action)
        {
            SynchronizationContext.Current.Post(_ => action(), null);
        }

        public void OnUserLoggedOut()
        {
            _loggedIn = false;
        }

        public void OnUserLoggedIn()
        {
            _loggedIn = true;
        }
    }
}
