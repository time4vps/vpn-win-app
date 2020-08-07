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

using System.Runtime.Serialization;

namespace T4VPN.Service.Contract.Vpn
{
    [DataContract]
    public class VpnStateContract
    {
        public VpnStateContract(
            VpnStatusContract status,
            VpnErrorTypeContract error,
            string endpointIp,
            bool networkBlocked,
            VpnProtocolContract protocol)
        {
            Status = status;
            Error = error;
            EndpointIp = endpointIp;
            NetworkBlocked = networkBlocked;
            Protocol = protocol;
        }

        [DataMember]
        public VpnStatusContract Status { get; private set; }

        [DataMember]
        public VpnErrorTypeContract Error { get; private set; }

        [DataMember]
        public bool NetworkBlocked { get; private set; }

        [DataMember]
        public string EndpointIp { get; private set; }

        [DataMember]
        public VpnProtocolContract Protocol { get; private set; }
    }
}
