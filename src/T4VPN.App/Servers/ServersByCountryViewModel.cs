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

using T4VPN.Common.Vpn;
using T4VPN.Core.Servers;
using T4VPN.Core.Servers.Models;
using T4VPN.Core.Servers.Specs;
using T4VPN.Core.Vpn;
using System.Collections.ObjectModel;

namespace T4VPN.Servers
{
    internal class ServersByCountryViewModel : BaseServerCollection
    {
        private readonly sbyte _userTier;
        private readonly ServerManager _serverManager;
        private readonly VpnState _vpnConnectionStatus;

        private bool _tor;
        private bool _p2P;

        public bool Tor
        {
            get => _tor;
            set => Set(ref _tor, value);
        }

        public bool P2P
        {
            get => _p2P;
            set => Set(ref _p2P, value);
        }

        public ServersByCountryViewModel(
            string countryCode,
            sbyte userTier,
            ServerManager serverManager,
            VpnState vpnConnectionStatus)
        {
            CountryCode = countryCode;
            _serverManager = serverManager;
            _vpnConnectionStatus = vpnConnectionStatus;
            _userTier = userTier;
        }

        public override void LoadServers()
        {
            var spec = new EntryCountryServer(CountryCode) && new StandardServer();
            var list = _serverManager.GetServers(spec);
            var collection = new ObservableCollection<IServerListItem>();

            foreach (var server in list)
            {
                if (!Tor)
                    Tor = ServerFeatures.SupportsTor(server.Features);
                if (!P2P)
                    P2P = ServerFeatures.SupportsP2P(server.Features);

                var item = new ServerItemViewModel(server, _userTier);
                item.OnVpnStateChanged(_vpnConnectionStatus);
                collection.Add(item);
            }

            Servers = collection;
        }

        public override void OnVpnStateChanged(VpnState state)
        {
            Connected = state.Status.Equals(VpnStatus.Connected)
                && state.Server is Server server
                && server.EntryCountry.Equals(CountryCode);

            foreach (var s in Servers)
            {
                s.OnVpnStateChanged(state);
            }
        }

        protected override bool HasAvailableServers()
        {
            if (!ServersAvailable.HasValue)
            {
                ServersAvailable = _serverManager.CountryHasAvailableServers(CountryCode, _userTier);
            }

            return ServersAvailable.Value;
        }
    }
}
