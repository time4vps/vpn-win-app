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

namespace T4VPN.Vpn.OpenVpn.Arguments
{
    internal class LocalHostArguments : IEnumerable<string>
    {
        private readonly string _localIp;

        public LocalHostArguments(string localIp)
        {
            _localIp = localIp;
        }

        public IEnumerator<string> GetEnumerator()
        {
            yield return $"--local {_localIp}";
            yield return "--lport 0";
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}