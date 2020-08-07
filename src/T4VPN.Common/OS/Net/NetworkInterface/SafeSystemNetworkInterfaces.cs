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
using System.Net.NetworkInformation;
using T4VPN.Common.Extensions;
using T4VPN.Common.Logging;

namespace T4VPN.Common.OS.Net.NetworkInterface
{
    public class SafeSystemNetworkInterfaces : INetworkInterfaces
    {
        private readonly ILogger _logger;
        private readonly INetworkInterfaces _origin;

        public SafeSystemNetworkInterfaces(ILogger logger, INetworkInterfaces origin)
        {
            _logger = logger;
            _origin = origin;
        }

        public event EventHandler NetworkAddressChanged
        {
            add => _origin.NetworkAddressChanged += value; 
            remove => _origin.NetworkAddressChanged -= value;
        }

        public INetworkInterface[] Interfaces()
        {
            try
            {
                return _origin.Interfaces();
            }
            catch (NetworkInformationException ex)
            {
                _logger.Error($"Failed to retrieve a list of system network interfaces: {ex.CombinedMessage()}");
                return new INetworkInterface[0];
            }
        }

        public INetworkInterface Interface(string interfaceDescription)
        {
            try
            {
                return _origin.Interface(interfaceDescription);
            }
            catch (NetworkInformationException ex)
            {
                _logger.Error($"Failed to retrieve a system network interface: {ex.CombinedMessage()}");
                return new NullNetworkInterface();
            }
        }
    }
}
