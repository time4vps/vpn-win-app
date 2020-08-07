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
using System.Collections.Generic;
using T4VPN.Common;
using T4VPN.Common.Vpn;
using T4VPN.Vpn.Common;

namespace T4VPN.Vpn.Connection
{
    /// <summary>
    /// Filters <see cref="StateChanged"/> events to not contain consequent events with
    /// the same status, error and remote IP value.
    /// A wrapper around <see cref="IVpnConnection"/>.
    /// </summary>
    public class FilteringStateWrapper : IVpnConnection
    {
        private readonly IVpnConnection _origin;

        private VpnState _prevState = new VpnState(VpnStatus.Disconnected);

        public FilteringStateWrapper(IVpnConnection origin)
        {
            _origin = origin;

            _origin.StateChanged += Origin_StateChanged;
        }

        public event EventHandler<EventArgs<VpnState>> StateChanged;

        public InOutBytes Total => _origin.Total;

        public void Connect(IReadOnlyList<VpnHost> servers, VpnConfig config, VpnProtocol protocol, VpnCredentials credentials)
        {
            _origin.Connect(servers, config, protocol, credentials);
        }

        public void Disconnect(VpnError error)
        {
            _origin.Disconnect(error);
        }

        public void UpdateServers(IReadOnlyList<VpnHost> servers, VpnConfig config)
        {
            _origin.UpdateServers(servers, config);
        }

        private void Origin_StateChanged(object sender, EventArgs<VpnState> e)
        {
            OnStateChanged(e.Data);
        }

        private void OnStateChanged(VpnState state)
        {
            if (state.Status != _prevState.Status || 
                state.Error != _prevState.Error ||
                !string.Equals(state.RemoteIp, _prevState.RemoteIp))
            {
                StateChanged?.Invoke(this, new EventArgs<VpnState>(state));
            }

            _prevState = state;
        }
    }
}