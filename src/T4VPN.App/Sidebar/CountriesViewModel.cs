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

using Caliburn.Micro;
using GalaSoft.MvvmLight.Command;
using T4VPN.Common.Vpn;
using T4VPN.Core.Auth;
using T4VPN.Core.Modals;
using T4VPN.Core.Servers;
using T4VPN.Core.Servers.Models;
using T4VPN.Core.Settings;
using T4VPN.Core.User;
using T4VPN.Core.Vpn;
using T4VPN.Onboarding;
using T4VPN.Resources;
using T4VPN.Servers;
using T4VPN.Trial;
using T4VPN.Vpn.Connectors;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace T4VPN.Sidebar
{
    internal class CountriesViewModel :
        Screen,
        IVpnPlanAware,
        IVpnStateAware,
        ISettingsAware,
        IOnboardingStepAware,
        ITrialStateAware,
        ILogoutAware,
        IServersAware
    {
        private readonly IAppSettings _appSettings;
        private readonly ServerListFactory _serverListFactory;
        private readonly App _app;
        private readonly IDialogs _dialogs;
        private readonly ServerConnector _serverConnector;
        private readonly CountryConnector _countryConnector;

        private VpnStateChangedEventArgs _vpnState = new VpnStateChangedEventArgs(new VpnState(VpnStatus.Disconnected), VpnError.None, false);

        public CountriesViewModel(
            IAppSettings appSettings,
            ServerListFactory serverListFactory,
            App app,
            IDialogs dialogs,
            ServerConnector serverConnector,
            CountryConnector countryConnector)
        {
            _appSettings = appSettings;
            _serverListFactory = serverListFactory;
            _app = app;
            _dialogs = dialogs;
            _serverConnector = serverConnector;
            _countryConnector = countryConnector;

            Connect = new RelayCommand<ServerItemViewModel>(ConnectAction);
            ConnectCountry = new RelayCommand<IServerCollection>(ConnectCountryAction);
            Expand = new RelayCommand<IServerCollection>(ExpandAction);
            ToggleSecureCoreCommand = new RelayCommand(ToggleSecureCoreAction);
            ClearSearchCommand = new RelayCommand(ClearSearchAction);
        }

        public ICommand Connect { get; }
        public ICommand ConnectCountry { get; }
        public ICommand Expand { get; set; }
        public ICommand ToggleSecureCoreCommand { get; }
        public ICommand ClearSearchCommand { get; }

        private bool _searchNotEmpty;

        public bool SearchNotEmpty
        {
            get => _searchNotEmpty;
            set => Set(ref _searchNotEmpty, value);
        }

        private string _searchValue;
        public string SearchValue
        {
            get => _searchValue;
            set
            {
                if (Set(ref _searchValue, value))
                {
                    CreateList();
                    SearchNotEmpty = !string.IsNullOrEmpty(_searchValue);
                }
            }
        }

        private bool _secureCore;
        [SuppressMessage("ReSharper", "ValueParameterNotUsed")]
        public bool SecureCore
        {
            get => _secureCore;
            set { }
        }

        private bool _showFifthOnboardingStep;
        public bool ShowFifthOnboardingStep
        {
            get => _showFifthOnboardingStep;
            set => Set(ref _showFifthOnboardingStep, value);
        }

        private ObservableCollection<IServerListItem> _items = new ObservableCollection<IServerListItem>();
        public ObservableCollection<IServerListItem> Items
        {
            get => _items;
            set => Set(ref _items, value);
        }

        public void ExpandCollection(IServerCollection serverCollection)
        {
            if (serverCollection.Expanded)
            {
                return;
            }
            serverCollection.Expanded = true;

            var index = _items.IndexOf(serverCollection) + 1;
            _app.Dispatcher?.Invoke(() =>
            {
                var collection = new ObservableCollection<IServerListItem>(serverCollection.Servers.Reverse());
                foreach (var serverListItem in collection)
                {
                    if (serverListItem is ServerItemViewModel server)
                    {
                        _items.Insert(index, server);
                    }
                }
            });
        }

        public void CollapseCollection(IServerCollection serverCollection)
        {
            if (!serverCollection.Expanded)
            {
                return;
            }
            serverCollection.Expanded = false;

            foreach (var item in serverCollection.Servers)
            {
                if (item is ServerItemViewModel server)
                {
                    _items.Remove(server);
                }
            }
        }

        public void Load()
        {
            SetSecureCore(_appSettings.SecureCore);
            CreateList();
        }

        public void OnServersUpdated()
        {
            CreateList();
            if (!State.Status.Equals(VpnStatus.Connected) ||
                !(State.Server is Server server))
                return;

            ExpandCountry(server.EntryCountry);
            UpdateVpnState(State);

            if (server.IsSecureCore())
                NotifyScServerRowsOfConnectedState(server, true);
            else
                NotifyServerRowsOfConnectedState(server, true);
        }

        public void OnAppSettingsChanged(PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(IAppSettings.SecureCore)))
            {
                SetSecureCore(_appSettings.SecureCore);
                CreateList();
            }
            else if (e.PropertyName.Equals(nameof(IAppSettings.Language)))
            {
                CreateList();
            }
        }

        public Task OnVpnStateChanged(VpnStateChangedEventArgs e)
        {
            _vpnState = e;

            if (!(e.State.Server is Server server))
                return Task.CompletedTask;

            if (e.State.Status == VpnStatus.Connected)
            {
                if (server.IsSecureCore())
                {
                    ExpandScCountry(server.ExitCountry);
                    UpdateVpnState(e.State);
                    NotifyScServerRowsOfConnectedState(server, true);
                }
                else
                {
                    ExpandCountry(server.EntryCountry);
                    UpdateVpnState(e.State);
                    NotifyServerRowsOfConnectedState(server, true);
                }
            }
            else
            {
                UpdateVpnState(e.State);
            }

            return Task.CompletedTask;
        }

        public Task OnVpnPlanChangedAsync(string plan)
        {
            CreateList();
            return Task.CompletedTask;
        }

        public void OnStepChanged(int step)
        {
            ShowFifthOnboardingStep = step == 5;
        }

        public Task OnTrialStateChangedAsync(PlanStatus status)
        {
            if (status == PlanStatus.Expired)
                _appSettings.SecureCore = false;

            return Task.CompletedTask;
        }

        public void OnUserLoggedOut()
        {
            _searchValue = string.Empty;
            NotifyOfPropertyChange(nameof(SearchValue));
        }

        private async void ToggleSecureCoreAction()
        {
            var value = !SecureCore;
            if (!await AllowToChangeSecureCore(value))
            {
                return;
            }

            _appSettings.SecureCore = value;
        }

        private void SetSecureCore(bool value)
        {
            Set(ref _secureCore, value, nameof(SecureCore));
        }

        private void CreateList()
        {
            Items = SecureCore
                ? _serverListFactory.BuildSecureCoreList()
                : _serverListFactory.BuildServerList(SearchValue);

            foreach (var item in Items.ToList())
            {
                if (item is ServersByCountryViewModel serversByCountry && serversByCountry.Expanded)
                {
                    serversByCountry.Expanded = false;
                    ExpandCollection(serversByCountry);
                }
            }

            if (_vpnState != null)
                OnVpnStateChanged(_vpnState);
        }

        private async Task<bool> AllowToChangeSecureCore(bool value)
        {
            if (!value && (State.Status.Equals(VpnStatus.Connecting) ||
                           State.Status.Equals(VpnStatus.Reconnecting) ||
                           State.Status.Equals(VpnStatus.Connected)))
            {
                var result = _dialogs.ShowQuestion(StringResources.Get("Sidebar_Countries_msg_SecureCoreDisableConfirm"));
                if (result.HasValue && result.Value)
                {
                    await _serverConnector.Disconnect();
                }
                else
                {
                    return false;
                }
            }

            if (value && (State.Status.Equals(VpnStatus.Connecting) ||
                          State.Status.Equals(VpnStatus.Reconnecting) ||
                          State.Status.Equals(VpnStatus.Connected)))
            {
                var result = _dialogs.ShowQuestion(StringResources.Get("Sidebar_Countries_msg_SecureCoreEnableConfirm"));
                if (result.HasValue && result.Value)
                {
                    await _serverConnector.Disconnect();
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        private void NotifyServerRowsOfConnectedState(Server server, bool connected)
        {
            var serverRows = Items.OfType<ServerItemViewModel>()
                .Where(c => c.Server.EntryCountry.Equals(server.EntryCountry));

            foreach (var row in serverRows)
            {
                row.SetConnectedToCountry(connected);
            }
        }

        private void NotifyScServerRowsOfConnectedState(Server server, bool connected)
        {
            var serverRows = Items.OfType<SecureCoreItemViewModel>()
                .Where(c => c.Server.IsSecureCore() && c.Server.ExitCountry.Equals(server.ExitCountry));

            foreach (var row in serverRows)
            {
                row.SetConnectedToCountry(connected);
            }
        }

        private void UpdateVpnState(VpnState state)
        {
            if (Items == null)
                return;

            foreach (var item in Items)
            {
                if (item is ServersByCountryViewModel || item is ServersByExitNodeViewModel)
                {
                    item.OnVpnStateChanged(state);
                }
            }
        }

        private void ExpandCountry(string countryCode)
        {
            var countryRow = Items.OfType<ServersByCountryViewModel>()
                .FirstOrDefault(c => c.CountryCode.Equals(countryCode));
            if (countryRow != null)
            {
                ExpandCollection(countryRow);
            }
        }

        private void ExpandScCountry(string countryCode)
        {
            var countryRow = Items.OfType<ServersByExitNodeViewModel>()
                .FirstOrDefault(c => c.CountryCode.Equals(countryCode));
            if (countryRow != null)
            {
                ExpandCollection(countryRow);
            }
        }

        private void ExpandAction(IServerCollection serverCollection)
        {
            if (!serverCollection.Expanded)
            {
                ExpandCollection(serverCollection);
            }
            else
            {
                CollapseCollection(serverCollection);
            }
        }

        private async void ConnectAction(ServerItemViewModel serverItemViewModel)
        {
            if (serverItemViewModel.Connecting || serverItemViewModel.Connected)
            {
                await _serverConnector.Disconnect();
                return;
            }

            await _serverConnector.Connect(serverItemViewModel.Server);
        }

        private async void ConnectCountryAction(IServerCollection serverCollection)
        {
            var currentServer = State.Server;

            if (State.Status.Equals(VpnStatus.Connected) &&
                serverCollection.CountryCode.Equals(currentServer?.ExitCountry))
            {
                await _countryConnector.Disconnect();
            }
            else
            {
                await _countryConnector.Connect(serverCollection.CountryCode);
            }
        }

        private void ClearSearchAction()
        {
            SearchValue = string.Empty;
        }

        private VpnState State => _vpnState.State;
    }
}
