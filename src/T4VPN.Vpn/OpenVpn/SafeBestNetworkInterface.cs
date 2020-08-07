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

using System.Net;
using System.Net.Sockets;

namespace T4VPN.Vpn.OpenVpn
{
    internal class SafeBestNetworkInterface
    {
        private readonly BestNetworkInterface _origin;

        public SafeBestNetworkInterface(BestNetworkInterface origin)
        {
            _origin = origin;
        }

        public IPAddress Ip()
        {
            try
            {
                return _origin.Ip();
            }
            catch (SocketException)
            {
                return null;
            }
        }
    }
}
