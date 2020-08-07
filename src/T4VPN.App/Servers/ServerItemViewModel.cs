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

using System.Collections.Generic;
using T4VPN.Common.Vpn;
using T4VPN.ConnectionInfo;
using T4VPN.Core.MVVM;
using T4VPN.Core.Servers;
using T4VPN.Core.Servers.Models;
using T4VPN.Core.Servers.Name;
using T4VPN.Core.Vpn;

namespace T4VPN.Servers
{
    internal class ServerItemViewModel : ViewModel, IServerListItem
    {
        private bool _connecting;
        private bool _connected;
        private bool _connectedToCountry;
        private bool _showIp = true;
        private string _ip;
        private readonly sbyte _userTier;

        public ConnectionInfoViewModel ConnectionInfoViewModel { get; }

        public string Ip
        {
            get => _ip;
            set => Set(ref _ip, value);
        }

        public bool Connecting
        {
            get => _connecting;
            set => Set(ref _connecting, value);
        }

        public bool Connected
        {
            get => _connected;
            set => Set(ref _connected, value);
        }

        public bool ConnectedToCountry
        {
            get => _connectedToCountry;
            set => Set(ref _connectedToCountry, value);
        }

        public bool ShowIp
        {
            get => _showIp;
            set => Set(ref _showIp, value);
        }

        public Server Server { get; set; }

        public string Name => Server.Name;

        public IName ConnectionName { get; set; }

        private bool _upgradeRequired;
        public bool UpgradeRequired
        {
            get => _upgradeRequired;
            set => Set(ref _upgradeRequired, value);
        }

        public List<IServerFeature> Features { get; set; }

        public ServerItemViewModel(Server server, sbyte userTier)
        {
            _userTier = userTier;
            AssignServer(server);
            SetServerFeatures(server);
            ConnectionInfoViewModel = new ConnectionInfoViewModel(server);
        }

        public void SetServerFeatures(Server server)
        {
            var list = new List<IServerFeature>();
            if (ServerFeatures.SupportsP2P(server.Features))
                list.Add(new P2PFeature());

            if (ServerFeatures.SupportsTor(server.Features))
                list.Add(new TorFeature());

            if (server.Tier.Equals(ServerTiers.Plus))
                list.Add(new PlusFeature());

            Features = list;
        }

        public virtual void OnVpnStateChanged(VpnState state)
        {
            if (state.Server == null || !state.Server.Equals(Server))
            {
                SetDisconnected();
                return;
            }

            switch (state.Status)
            {
                case VpnStatus.Connected:
                    SetConnected();
                    break;
                case VpnStatus.Connecting:
                case VpnStatus.Reconnecting:
                    SetConnecting();
                    break;
                default:
                    SetDisconnected();
                    break;
            }
        }

        public void SetConnectedToCountry(bool connected)
        {
            ConnectedToCountry = connected;
        }

        private void AssignServer(Server server)
        {
            Server = server;
            Ip = server.ExitIp;

            ConnectionName = Server.GetServerName();
            SetIpVisibility(server);
            UpgradeRequired = _userTier < server.Tier;
            ConnectionName.UpgradeRequired = UpgradeRequired;
        }

        private void SetIpVisibility(Server server)
        {
            ShowIp = server.Tier > ServerTiers.Free;
        }

        protected void SetDisconnected()
        {
            Connecting = false;
            Connected = false;
            SetConnectedToCountry(false);
        }

        protected void SetConnecting()
        {
            Connecting = true;
            Connected = false;
            SetConnectedToCountry(false);
        }

        protected void SetConnected()
        {
            Connected = true;
            Connecting = false;
        }

        public bool MatchesQuery(string query = null)
        {
            return !string.IsNullOrEmpty(query) && Name.ToLower().Contains(query);
        }
    }
}
