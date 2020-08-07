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

using System.Collections;
using System.Collections.Generic;
using T4VPN.Common.Configuration;

namespace T4VPN.Vpn.OpenVpn.Arguments
{
    internal class ManagementArguments : IEnumerable<string>
    {
        private readonly OpenVpnConfig _config;
        private readonly int _managementPort;

        public ManagementArguments(OpenVpnConfig config, int managementPort)
        {
            _config = config;
            _managementPort = managementPort;
        }

        public IEnumerator<string> GetEnumerator()
        {
            yield return $"--management {_config.ManagementHost} {_managementPort} stdin";
            yield return "--management-query-passwords";
            yield return "--management-hold";
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
