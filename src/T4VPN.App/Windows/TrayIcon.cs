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

using Caliburn.Micro;
using T4VPN.Common.Vpn;
using T4VPN.Core.Auth;
using T4VPN.Core.Events;
using T4VPN.Core.Vpn;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;

namespace T4VPN.Windows
{
    public class TrayIcon :
        IVpnStateAware,
        ILogoutAware,
        ILoggedInAware,
        IHandle<ShowNotificationMessage>
    {
        private readonly NotifyIcon _nIcon = new NotifyIcon();
        private bool _toggle;
        private bool _connecting;
        private readonly DispatcherTimer _timer = new DispatcherTimer();

        private readonly ResourceIcon _connected;
        private readonly ResourceIcon _notConnected;
        private readonly ResourceIcon _balloonIcon;
        private readonly BalloonNotification _balloonNotification;
        private readonly TrayIconMouse _trayIconMouse;

        private const string ConnectedIconPath = "Resources/Assets/Images/Icons/systray-connected.ico";
        private const string NotConnectedIconPath = "Resources/Assets/Images/Icons/systray-notconnected.ico";
        private const string AppIconPath = "t4vpn.ico";

        public TrayIcon(
            IEventAggregator eventAggregator,
            BalloonNotification balloonNotification,
            TrayIconMouse trayIconMouse)
        {
            _trayIconMouse = trayIconMouse;
            _balloonNotification = balloonNotification;
            eventAggregator.Subscribe(this);

            _timer.Interval = TimeSpan.FromSeconds(0.5);
            _timer.Tick += OnTimerTick;
            _nIcon.MouseDown += trayIconMouse.TrayIconClick;

            _connected = new ResourceIcon(ConnectedIconPath);
            _notConnected = new ResourceIcon(NotConnectedIconPath);
            _balloonIcon = new ResourceIcon(AppIconPath);
        }

        public event MouseEventHandler OnMouseEvent
        {
            add => _trayIconMouse.OnMouseEvent += value;
            remove => _trayIconMouse.OnMouseEvent -= value;
        }

        public Task OnVpnStateChanged(VpnStateChangedEventArgs e)
        {
            if (e.State.Status == VpnStatus.Connecting || e.State.Status == VpnStatus.Reconnecting)
            {
                _nIcon.Icon = _notConnected.Value();
                _connecting = true;
                _timer.Start();
            }

            if (e.State.Status == VpnStatus.Disconnecting || e.State.Status == VpnStatus.Disconnected)
            {
                _timer.Stop();
                _nIcon.Icon = _notConnected.Value();
                _connecting = false;
            }

            if (e.State.Status == VpnStatus.Connected)
            {
                _nIcon.Icon = _connected.Value();
                _connecting = false;
                _timer.Stop();
            }

            return Task.CompletedTask;
        }

        public void ShowBalloon(string message)
        {
            _balloonNotification.Show(_nIcon, _balloonIcon.Value(new Size(256, 256)), message, 5000);
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            ToggleConnectingIcon();
        }

        private void ToggleConnectingIcon()
        {
            if (_connecting)
            {
                _toggle = !_toggle;
                SetIcon(_toggle ? VpnStatus.Connecting : VpnStatus.Disconnected);
            }
        }

        private void SetIcon(VpnStatus status)
        {
            switch (status)
            {
                case VpnStatus.Connected:
                case VpnStatus.Connecting:
                    _nIcon.Icon = _connected.Value();
                    break;
                default:
                    _nIcon.Icon = _notConnected.Value();
                    break;
            }
        }

        public void OnUserLoggedIn()
        {
            _nIcon.Icon = _notConnected.Value();
            _nIcon.Visible = true;
        }

        public void OnUserLoggedOut()
        {
            Hide();
        }

        public void Hide()
        {
            _nIcon.Visible = false;
            _nIcon.Icon = _notConnected.Value();
        }

        public void Handle(ShowNotificationMessage message)
        {
            ShowBalloon(message.Text);
        }
    }
}
