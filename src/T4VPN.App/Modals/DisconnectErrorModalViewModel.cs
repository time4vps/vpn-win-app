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

using GalaSoft.MvvmLight.CommandWpf;
using T4VPN.BugReporting;
using T4VPN.Common.Logging;
using T4VPN.Common.Vpn;
using T4VPN.Config.Url;
using T4VPN.ConnectionInfo;
using T4VPN.Core.Modals;
using T4VPN.Core.Service.Vpn;
using T4VPN.Settings;
using System.Windows.Input;

namespace T4VPN.Modals
{
    public class DisconnectErrorModalViewModel : BaseModalViewModel
    {
        private VpnError _error;
        private bool _networkBlocked;
        private readonly IActiveUrls _urlConfig;
        private readonly ConnectionErrorResolver _connectionErrorResolver;
        private readonly IVpnManager _vpnManager;
        private readonly SettingsModalViewModel _settingsModalViewModel;
        private readonly IModals _modals;
        private readonly ILogger _logger;

        public ICommand OpenHelpArticleCommand { get; set; }
        public ICommand SettingsCommand { get; set; }
        public ICommand DisableKillSwitchCommand { get; set; }
        public ICommand ReportBugCommand { get; set; }
        public ICommand GoToAccountCommand { get; set; }

        public DisconnectErrorModalViewModel(
            ILogger logger,
            IActiveUrls urlConfig,
            ConnectionErrorResolver connectionErrorResolver,
            IVpnManager vpnManager,
            IModals modals,
            SettingsModalViewModel settingsModalViewModel)
        {
            _logger = logger;
            _modals = modals;
            _settingsModalViewModel = settingsModalViewModel;
            _vpnManager = vpnManager;
            _connectionErrorResolver = connectionErrorResolver;
            _urlConfig = urlConfig;
            OpenHelpArticleCommand = new RelayCommand(OpenHelpArticleAction);
            SettingsCommand = new RelayCommand(OpenSettings);
            DisableKillSwitchCommand = new RelayCommand(DisableKillSwitch);
            ReportBugCommand = new RelayCommand(ReportBug);
            GoToAccountCommand = new RelayCommand(OpenAccountPage);
        }

        public VpnError Error
        {
            get => _error;
            set => Set(ref _error, value);
        }

        public bool NetworkBlocked
        {
            get => _networkBlocked;
            set => Set(ref _networkBlocked, value);
        }

        public override void BeforeOpenModal(dynamic options)
        {
            if (options == null)
                return;

            NetworkBlocked = options.NetworkBlocked;
            Error = options.Error;

            _logger.Info($"Disconnected due to: {Error}. Network blocked: {NetworkBlocked}");
        }

        protected override async void OnViewReady(object view)
        {
            base.OnViewReady(view);

            if (Error == VpnError.AuthorizationError)
            {
                var error = await _connectionErrorResolver.ResolveError();
                if (error == VpnError.PasswordChanged)
                {
                    TryClose(true);
                    await _vpnManager.Reconnect();
                }
                else
                {
                    Error = error;
                }
            }
        }

        private void OpenHelpArticleAction()
        {
            _urlConfig.TroubleShootingUrl.Open();
        }

        private void OpenSettings()
        {
            TryClose();
            _settingsModalViewModel.OpenAdvancedTab();
            _modals.Show<SettingsModalViewModel>();
        }

        private void OpenAccountPage()
        {
            _urlConfig.AccountUrl.Open();
        }

        private async void DisableKillSwitch()
        {
            TryClose();
            await _vpnManager.Disconnect();
            _logger.Info("Killswitch disabled");
        }

        private void ReportBug()
        {
            TryClose();
            _modals.Show<ReportBugModalViewModel>();
        }
    }
}
