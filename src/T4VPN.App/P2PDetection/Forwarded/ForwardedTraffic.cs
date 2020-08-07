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

using System.Linq;
using System.Threading.Tasks;
using T4VPN.Config;
using T4VPN.Core.Servers.Models;
using T4VPN.Core.User;
using T4VPN.Core.Vpn;


namespace T4VPN.P2PDetection.Forwarded
{
    /// <summary>
    ///     Detects forwarded traffic on not free servers.
    /// </summary>
    /// <remarks>
    ///     If P2P traffic is detected on not free VPN server, on which internet provider does not support P2P,
    ///     all traffic is forwarded to another exit server, where internet provider supports P2P.
    ///     It is considered the traffic to be forwarded when current user IP does not match VPN server IP and
    ///     current IP is one of the black hole predefined IPs.
    /// </remarks>
    internal class ForwardedTraffic : IForwardedTraffic, IVpnStateAware
    {
        private readonly IVpnConfig _config;
        private readonly IUserLocationService _userLocationService;
        private Server _server = Server.Empty();

        public ForwardedTraffic(IUserLocationService userLocationService, IVpnConfig config)
        {
            _config = config;
            _userLocationService = userLocationService;
        }

        public async Task<ForwardedTrafficResult> Value()
        {
            var response = await _userLocationService.LocationAsync();
            if (response.Failure)
            {
                return new ForwardedTrafficResult(false, false, string.Empty);
            }

            return new ForwardedTrafficResult(true, IsForwarded(response.Value.Ip), response.Value.Ip);
        }

        public Task OnVpnStateChanged(VpnStateChangedEventArgs e)
        {
            _server = e.State.Server;

            return Task.CompletedTask;
        }

        private bool IsForwarded(string ip)
        {
            if (_server.Equals(Server.Empty()))
            {
                return false;
            }

            return ip != _server.ExitIp && _config.BlackHoleIps.Contains(ip);
        }
    }
}