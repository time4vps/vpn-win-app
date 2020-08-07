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

using System;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows.Threading;
using T4VPN.Common.Threading;
using T4VPN.Common.Vpn;
using T4VPN.Core.Vpn;

namespace T4VPN.Core.Service.Vpn
{
    internal class VpnConnectionSpeed: IVpnStateAware
    {
        private readonly SerialTaskQueue _lock = new SerialTaskQueue();
        private readonly IVpnServiceManager _vpnServiceManager;
        private readonly DispatcherTimer _timer;

        private InOutBytes _total;
        private InOutBytes _speed;

        public VpnConnectionSpeed(IVpnServiceManager vpnServiceManager)
        {
            _vpnServiceManager = vpnServiceManager;

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += UpdateSpeed;
        }

        public VpnSpeed Speed()
        {
            return new VpnSpeed(Math.Max(0, _speed.BytesIn), Math.Max(0, _speed.BytesOut));
        }

        public double TotalDownloaded()
        {
            return _total.BytesIn;
        }

        public double TotalUploaded()
        {
            return _total.BytesOut;
        }

        public Task OnVpnStateChanged(VpnStateChangedEventArgs e)
        {
            var connected = e.State.Status == VpnStatus.Connected;

            if (connected)
                _timer.Start();
            else
                _timer.Stop();

            return Task.CompletedTask;
        }

        private async void UpdateSpeed(object sender, EventArgs e)
        {
            InOutBytes total;
            using (await _lock.Lock())
            {
                try
                {
                    total = await _vpnServiceManager.Total();
                }
                catch (CommunicationException)
                {
                    return;
                }
                catch (TimeoutException)
                {
                    return;
                }
            }

            _speed = total - _total;
            _total = total;
        }
    }
}
