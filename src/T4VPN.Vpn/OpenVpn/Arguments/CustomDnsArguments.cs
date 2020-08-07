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

using System.Collections;
using System.Collections.Generic;

namespace T4VPN.Vpn.OpenVpn.Arguments
{
    internal class CustomDnsArguments : IEnumerable<string>
    {
        private readonly IReadOnlyCollection<string> _dns;

        public CustomDnsArguments(IReadOnlyCollection<string> dns)
        {
            _dns = dns;
        }

        public IEnumerator<string> GetEnumerator()
        {
            if (_dns.Count == 0)
            {
                yield break;
            }

            yield return "--pull-filter ignore \"dhcp-option DNS\"";

            foreach (var dns in _dns)
            {
                yield return $"--dhcp-option DNS {dns}";
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
