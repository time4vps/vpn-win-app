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

using T4VPN.Common.Extensions;
using T4VPN.Common.Helpers;
using System;

namespace T4VPN.Common.Vpn
{
    public struct VpnHost
    {
        public VpnHost(string name, string ip)
        {
            AssertHostNameIsValid(name);
            AssertIpAddressIsValid(ip);

            Name = name;
            Ip = ip;
        }

        public string Name { get; }

        public string Ip { get; }

        public bool IsEmpty() => string.IsNullOrEmpty(Name) && string.IsNullOrEmpty(Ip);

        private static void AssertHostNameIsValid(string hostName)
        {
            Ensure.NotEmpty(hostName, nameof(hostName));

            var hostNameType = Uri.CheckHostName(hostName);
            if (hostNameType != UriHostNameType.Dns && hostNameType != UriHostNameType.IPv4)
            {
                throw new ArgumentException($"Invalid argument {nameof(hostName)} value: {hostName}");
            }
        }

        private static void AssertIpAddressIsValid(string ip)
        {
            Ensure.NotEmpty(ip, nameof(ip));

            if (!ip.IsValidIpAddress())
            {
                throw new ArgumentException($"Invalid argument {nameof(ip)} value: {ip}");
            }
        }
    }
}
