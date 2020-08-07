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

using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using T4VPN.Common.Vpn;
using T4VPN.Core.Modals;
using T4VPN.Core.Vpn;
using T4VPN.Resources;

namespace T4VPN.Login.Views
{
    public partial class LoginWindow
    {
        private bool _networkBlocked;
        private readonly IDialogs _dialogs;

        public LoginWindow(IDialogs dialogs)
        {
            _dialogs = dialogs;
            InitializeComponent();

            CloseButton.Click += CloseButton_Click;
            MinimizeButton.Click += Minimize_Click;
        }

        public Task OnVpnStateChanged(VpnStateChangedEventArgs e)
        {
            _networkBlocked = e.NetworkBlocked &&
                              (e.State.Status == VpnStatus.Disconnecting ||
                               e.State.Status == VpnStatus.Disconnected);

            return Task.CompletedTask;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;

            if (_networkBlocked)
            {
                var result = _dialogs.ShowQuestion(StringResources.Get("Login_msg_ExitKillSwitchConfirm"));
                if (!result.HasValue || !result.Value)
                {
                    return;
                }
            }

            base.OnClosing(e);
            Application.Current.Shutdown();
        }
    }
}
