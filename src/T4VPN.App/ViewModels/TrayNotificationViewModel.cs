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

using GalaSoft.MvvmLight.Command;
using T4VPN.Common.Vpn;
using T4VPN.Core;
using T4VPN.Core.MVVM;
using T4VPN.Core.Service.Vpn;
using T4VPN.Core.Vpn;
using T4VPN.QuickLaunch;
using T4VPN.Vpn.Connectors;
using System.Threading.Tasks;
using System.Windows.Input;

namespace T4VPN.ViewModels
{
    internal class TrayNotificationViewModel : ViewModel, IVpnStateAware
    {
        private readonly QuickConnector _quickConnector;
        private readonly IVpnManager _vpnManager;
        private readonly AppExitHandler _appExitHandler;

        public TrayNotificationViewModel(
            IVpnManager vpnManager,
            QuickConnector quickConnector,
            AppExitHandler appExitHandler,
            QuickLaunchViewModel quickLaunchViewModel)
        {
            _vpnManager = vpnManager;
            _quickConnector = quickConnector;
            _appExitHandler = appExitHandler;

            ExitCommand = new RelayCommand(ExitAction);
            DisconnectCommand = new RelayCommand(DisconnectAction);
            ConnectCommand = new RelayCommand(ConnectAction);

            HandleVpnStatus(VpnStatus.Disconnected);

            QuickLaunch = quickLaunchViewModel;
        }

        public ICommand ExitCommand { get; }
        public ICommand DisconnectCommand { get; }
        public ICommand ConnectCommand { get; }

        internal QuickLaunchViewModel QuickLaunch { get; set; }

        private bool _connecting;
        public bool Connecting
        {
            get => _connecting;
            set => Set(ref _connecting, value);
        }

        private bool _canConnect;
        public bool CanConnect
        {
            get => _canConnect;
            set => Set(ref _canConnect, value);
        }

        private bool _canDisconnect;
        public bool CanDisconnect
        {
            get => _canDisconnect;
            set => Set(ref _canDisconnect, value);
        }

        private VpnStatus _vpnStatus;
        public VpnStatus VpnStatus
        {
            get => _vpnStatus;
            set => Set(ref _vpnStatus, value);
        }

        public Task OnVpnStateChanged(VpnStateChangedEventArgs e)
        {
            VpnStatus = e.State.Status;
            HandleVpnStatus(e.State.Status);

            return Task.CompletedTask;
        }

        private void HandleVpnStatus(VpnStatus status)
        {
            switch (status)
            {
                case VpnStatus.AssigningIp:
                case VpnStatus.Authenticating:
                case VpnStatus.Waiting:
                case VpnStatus.RetrievingConfiguration:
                case VpnStatus.Connecting:
                case VpnStatus.Reconnecting:
                    Connecting = true;
                    CanConnect = false;
                    CanDisconnect = true;
                    break;
                case VpnStatus.Connected:
                    CanConnect = false;
                    CanDisconnect = true;
                    Connecting = false;
                    break;
                case VpnStatus.Disconnecting:
                case VpnStatus.Disconnected:
                    CanConnect = true;
                    CanDisconnect = false;
                    Connecting = false;
                    break;
            }
        }

        private void ExitAction()
        {
            _appExitHandler.RequestExit();
        }

        private async void ConnectAction()
        {
            await _quickConnector.Connect();
        }

        private async void DisconnectAction()
        {
            await _vpnManager.Disconnect();
        }
    }
}
