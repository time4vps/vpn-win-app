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

using T4VPN.Vpn.Common;
using System.Collections.Generic;
using T4VPN.Common.Vpn;

namespace T4VPN.Vpn.OpenVpn
{
    public class OpenVpnProcessParams
    {
        public OpenVpnProcessParams(
            VpnEndpoint endpoint,
            int managementPort,
            string password,
            VpnConfig config)            
        {
            Endpoint = endpoint;
            ManagementPort = managementPort;
            Password = password;
            CustomDns = config.CustomDns;
            SkipIpv6 = config.SkipIpv6;
        }

        public VpnEndpoint Endpoint { get; }
        public int ManagementPort { get; }
        public string Password { get; }
        public IReadOnlyCollection<string> CustomDns { get; }
        public bool SkipIpv6 { get; }
    }
}
