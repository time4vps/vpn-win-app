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

namespace T4VPN.Service.SplitTunneling
{
    public class ConnectRedirectData
    {
        private readonly IPAddress _ipAddress;

        public ConnectRedirectData(IPAddress ipAddress)
        {
            _ipAddress = ipAddress;
        }

        public byte[] Value() => _ipAddress.GetAddressBytes();

        public static implicit operator byte[](ConnectRedirectData item) => item.Value();
    }
}
