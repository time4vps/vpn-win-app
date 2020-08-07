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

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using T4VPN.Common.Vpn;
using T4VPN.Core.Vpn;

namespace T4VPN.Core.Service.Vpn
{
    public interface IVpnServiceManager
    {
        Task Connect(VpnConnectionRequest connectionRequest);

        Task UpdateServers(IReadOnlyList<VpnHost> servers, VpnConfig config);

        Task Disconnect(VpnError vpnError);

        Task<InOutBytes> Total();

        Task RepeatState();

        void RegisterCallback(Action<VpnStateChangedEventArgs> onVpnStateChanged);
    }
}
