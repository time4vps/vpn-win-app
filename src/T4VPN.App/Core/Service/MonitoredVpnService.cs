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

using T4VPN.Common.Abstract;
using T4VPN.Common.OS.Services;
using T4VPN.Common.Vpn;
using T4VPN.Core.Vpn;
using System;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace T4VPN.Core.Service
{
    internal class MonitoredVpnService : IVpnStateAware, IConcurrentService
    {
        private VpnStatus _vpnStatus;

        private readonly VpnSystemService _service;
        private readonly DispatcherTimer _timer = new DispatcherTimer();

        public MonitoredVpnService(Common.Configuration.Config appConfig, VpnSystemService service)
        {
            _service = service;

            _timer.Interval = appConfig.ServiceCheckInterval;
            _timer.Tick += OnTimerTick;
        }

        public event EventHandler<string> ServiceStarted
        {
            add => _service.ServiceStarted += value;
            remove => _service.ServiceStarted -= value;
        }

        public string Name => _service.Name;

        public bool Running() => _service.Running();

        public Task<Result> StartAsync() => _service.StartAsync();

        public Task<Result> StopAsync() => _service.StopAsync();

        public Task OnVpnStateChanged(VpnStateChangedEventArgs e)
        {
            _vpnStatus = e.State.Status;

            if (!_timer.IsEnabled && _vpnStatus != VpnStatus.Disconnected)
            {
                _timer.Start();
            }

            if (_timer.IsEnabled && _vpnStatus == VpnStatus.Disconnected)
            {
                _timer.Stop();
            }

            return Task.CompletedTask;
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            if (_vpnStatus == VpnStatus.Disconnected)
            {
                return;
            }

            if (!Running())
            {
                StartAsync();
            }
        }
    }
}
