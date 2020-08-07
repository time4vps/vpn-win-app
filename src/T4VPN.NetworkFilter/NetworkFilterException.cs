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

namespace T4VPN.NetworkFilter
{
    public class NetworkFilterException: Exception
    {
        public uint Code { get; }

        public NetworkFilterException(uint code) : base($"The IpFilter call returned error code {code}")
        {
            Code = code;
        }

        public NetworkFilterException(int code, Exception innerException) : base($"The IpFilter call failed with error code {code}", innerException)
        {
            unchecked
            {
                Code = (uint) code;
            }
        }
    }

    public class InvalidIpAddressException: NetworkFilterException
    {
        public InvalidIpAddressException(uint code): base(code)
        {
        }
    }

    public class NetInterfaceNotFoundException: NetworkFilterException
    {
        public NetInterfaceNotFoundException(uint code): base(code)
        {
        }
    }

    public class InvalidNetworkAddressException: NetworkFilterException
    {
        public InvalidNetworkAddressException(uint code): base(code)
        {
        }
    }

    public class FilterNotFoundException: NetworkFilterException
    {
        public FilterNotFoundException(uint code): base(code)
        {
        }
    }

    public class InUseException: NetworkFilterException
    {
        public InUseException(uint code): base(code)
        {
        }
    }
}
