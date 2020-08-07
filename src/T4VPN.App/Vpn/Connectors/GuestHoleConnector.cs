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
using System.Linq;
using System.Threading.Tasks;
using T4VPN.Common.Storage;
using T4VPN.Common.Vpn;
using T4VPN.Config;
using T4VPN.Core.Api;
using T4VPN.Core.Servers.Contracts;
using T4VPN.Core.Service.Vpn;
using T4VPN.Core.Vpn;
using VpnConfig = T4VPN.Common.Vpn.VpnConfig;

namespace T4VPN.Vpn.Connectors
{
    public class GuestHoleConnector
    {
        private int _reconnects;

        private readonly Random _random = new Random();

        private readonly IVpnServiceManager _vpnServiceManager;
        private readonly IVpnConfig _openVpnConfig;
        private readonly GuestHoleState _guestHoleState;
        private readonly Common.Configuration.Config _config;
        private readonly ICollectionStorage<GuestHoleServerContract> _guestHoleServers;

        public GuestHoleConnector(
            IVpnServiceManager vpnServiceManager,
            IVpnConfig openVpnConfig,
            GuestHoleState guestHoleState,
            Common.Configuration.Config config,
            ICollectionStorage<GuestHoleServerContract> guestHoleServers)
        {
            _guestHoleServers = guestHoleServers;
            _config = config;
            _guestHoleState = guestHoleState;
            _vpnServiceManager = vpnServiceManager;
            _openVpnConfig = openVpnConfig;
        }

        public async Task Connect()
        {
            var request = new VpnConnectionRequest(
                Servers(),
                VpnProtocol.Auto,
                VpnConfig(),
                new VpnCredentials(_config.GuestHoleVpnUsername, _config.GuestHoleVpnPassword));

            await _vpnServiceManager.Connect(request);
        }

        public async Task Disconnect()
        {
            await _vpnServiceManager.Disconnect(VpnError.NoneKeepEnabledKillSwitch);
        }

        public async Task OnVpnStateChanged(VpnStateChangedEventArgs e)
        {
            if (!_guestHoleState.Active)
            {
                return;
            }

            if (e.State.Status == VpnStatus.Connected)
            {
                _reconnects = 0;
                return;
            }

            if (e.State.Status == VpnStatus.Reconnecting)
            {
                _reconnects++;
            }

            if (_reconnects >= _config.MaxGuestHoleRetries)
            {
                _reconnects = 0;
                await Disconnect();
            }
        }

        public IReadOnlyList<VpnHost> Servers()
        {
            return _guestHoleServers
                .GetAll()
                .Select(server => new VpnHost(server.Host, server.Ip))
                .OrderBy(s => _random.Next())
                .ToList();
        }

        private VpnConfig VpnConfig()
        {
            var portConfig = new Dictionary<VpnProtocol, IReadOnlyCollection<int>>
            {
                { VpnProtocol.OpenVpnUdp, _openVpnConfig.UdpPorts },
                { VpnProtocol.OpenVpnTcp, _openVpnConfig.TcpPorts },
            };

            return new VpnConfig(portConfig, new List<string>());
        }
    }
}