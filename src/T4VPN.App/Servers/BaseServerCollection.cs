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

using T4VPN.Core.MVVM;
using T4VPN.Core.Vpn;
using System.Collections.ObjectModel;

namespace T4VPN.Servers
{
    internal abstract class BaseServerCollection : ViewModel, IServerCollection
    {
        private bool _connected;
        private bool _upgradeRequired;

        protected ObservableCollection<IServerListItem> ServersValue;
        protected bool ExpandedValue;
        protected bool? ServersAvailable;

        public string CountryCode { get; set; }

        public ObservableCollection<IServerListItem> Servers
        {
            get
            {
                if (ServersValue == null)
                {
                    LoadServers();
                }

                return ServersValue;
            }
            set
            {
                ServersValue = value;
                OnPropertyChanged();
            }
        }

        public bool Dimmed
        {
            get => !HasAvailableServers();
            set { }
        }

        public bool Expanded
        {
            get => ExpandedValue;
            set
            {
                if (ExpandedValue == value) return;
                ExpandedValue = value;
                OnPropertyChanged();
            }
        }

        public bool Connected
        {
            get => _connected;
            set => Set(ref _connected, value);
        }

        public bool UpgradeRequired
        {
            get => _upgradeRequired;
            set => Set(ref _upgradeRequired, value);
        }

        public string Name
        {
            get => Countries.GetName(CountryCode);
            set { }
        }

        public bool IsMarkedForRemoval { get; set; } = false;

        public abstract void LoadServers();

        public abstract void OnVpnStateChanged(VpnState state);

        public bool MatchesQuery(string query = null)
        {
            return !string.IsNullOrEmpty(query) && Name.ToLower().Contains(query);
        }

        protected abstract bool HasAvailableServers();
    }
}
