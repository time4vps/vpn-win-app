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

using System.ServiceModel;
using System.Threading.Tasks;
using T4VPN.Service.Contract.Settings;

namespace T4VPN.Service.Contract.Vpn
{
    [ServiceContract(CallbackContract = typeof(IVpnEventsContract))]
    public interface IVpnConnectionContract
    {
        [OperationContract(IsOneWay = true)]
        Task Connect(VpnConnectionRequestContract connectionRequest);

        [OperationContract(IsOneWay = true)]
        Task UpdateServers(VpnHostContract[] servers, VpnConfigContract config);

        [OperationContract(IsOneWay = true)]
        Task Disconnect(SettingsContract settings, VpnErrorTypeContract vpnError);

        [OperationContract(IsOneWay = true)]
        Task RegisterCallback();

        [OperationContract(IsOneWay = true)]
        Task UnRegisterCallback();

        [OperationContract(IsOneWay = false)]
        Task RepeatState();

        [OperationContract(IsOneWay = false)]
        Task<InOutBytesContract> Total();
    }
}
