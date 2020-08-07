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
using T4VPN.Common.Extensions;

namespace T4VPN.Common.Vpn
{
    public class VpnConfig
    {
        public IReadOnlyDictionary<VpnProtocol, IReadOnlyCollection<int>> Ports { get; }

        public IReadOnlyCollection<string> CustomDns { get; }

        public bool SkipIpv6 { get; set; } = false;

        public VpnConfig(
            IReadOnlyDictionary<VpnProtocol, IReadOnlyCollection<int>> portConfig,
            IReadOnlyCollection<string> customDns)
        {
            AssertPortsValid(portConfig);
            AssertCustomDnsValid(customDns);

            Ports = portConfig;
            CustomDns = customDns;
        }

        private void AssertCustomDnsValid(IReadOnlyCollection<string> customDns)
        {
            foreach (var dns in customDns)
            {
                if (!dns.IsValidIpAddress())
                {
                    throw new ArgumentException($"Invalid DNS address: {dns}");
                }
            }
        }

        private void AssertPortsValid(IReadOnlyDictionary<VpnProtocol, IReadOnlyCollection<int>> ports)
        {
            foreach (var item in ports)
            {
                foreach (var port in item.Value)
                {
                    if (port < 1 || port > 65535)
                    {
                        throw new ArgumentException($"Invalid OpenVPN port: {port}");
                    }
                }
            }
        }
    }
}
