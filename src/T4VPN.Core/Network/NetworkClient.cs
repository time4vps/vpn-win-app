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

using System;
using System.Linq;
using System.Net.NetworkInformation;

namespace T4VPN.Core.Network
{
    public class NetworkClient : INetworkClient
    {
        private readonly SafeWlanClient _client;
        private string _ssid = string.Empty;

        public NetworkClient()
        {
            _client = new SafeWlanClient(new WlanClient());

            NetworkChange.NetworkAddressChanged += NetworkAddressChanged;
        }

        public event EventHandler<WifiChangeEventArgs> WifiChangeDetected;

        public void CheckForInsecureWiFi()
        {
            NetworkAddressChanged(this, EventArgs.Empty);
        }

        private void NetworkAddressChanged(object sender, EventArgs e)
        {
            foreach (var connection in _client.GetActiveWifiConnections())
            {
                if (_ssid == connection.Name)
                    continue;

                _ssid = connection.Name;

                WifiChangeDetected?.Invoke(this, new WifiChangeEventArgs(connection.Name, connection.Secure));
            }

            var insecureWifi = _client.GetActiveWifiConnections().Any(i => !i.Secure);
            if (!insecureWifi)
                _ssid = string.Empty;
        }
    }
}
