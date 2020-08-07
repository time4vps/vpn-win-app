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

using System;
using System.Windows.Threading;
using T4VPN.Core.Auth;

namespace T4VPN.Core.Events
{
    public class EventTimer : ILogoutAware
    {
        private readonly DispatcherTimer _timer;
        private readonly EventClient _eventClient;

        public EventTimer(EventClient eventClient, TimeSpan interval)
        {
            _eventClient = eventClient;

            _timer = new DispatcherTimer
            {
                Interval = interval
            };
            _timer.Tick += OnTimerTick;
        }

        public void OnUserLoggedOut()
        {
            Stop();
        }

        public void Start()
        {
            _timer.Start();
        }

        private void Stop()
        {
            _timer.Stop();
        }

        private async void OnTimerTick(object sender, EventArgs e)
        {
            await _eventClient.GetEvents();
        }
    }
}
