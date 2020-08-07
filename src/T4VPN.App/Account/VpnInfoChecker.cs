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

using T4VPN.Core.Api;
using T4VPN.Core.Auth;
using T4VPN.Core.Settings;
using System;
using System.Net.Http;
using System.Threading;
using System.Windows.Threading;

namespace T4VPN.Account
{
    public class VpnInfoChecker : ILogoutAware
    {
        private readonly IApiClient _api;
        private readonly IUserStorage _userStorage;
        private readonly DispatcherTimer _timer;
        private static readonly SemaphoreSlim Semaphore = new SemaphoreSlim(1, 1);

        public VpnInfoChecker(IApiClient api, IUserStorage userStorage)
        {
            _api = api;
            _userStorage = userStorage;
            _timer = new DispatcherTimer();
            _timer.Tick += OnTimerTick;
        }

        public void Start(TimeSpan interval)
        {
            _timer.Interval = interval;
            _timer.Start();
        }

        public void OnUserLoggedOut()
        {
            if (_timer.IsEnabled)
                _timer.Stop();
        }

        private async void OnTimerTick(object sender, EventArgs e)
        {
            await Semaphore.WaitAsync();

            try
            {
                var response = await _api.GetVpnInfoResponse();
                if (response.Success)
                    _userStorage.StoreVpnInfo(response.Value);
            }
            catch (HttpRequestException)
            {
            }
            finally
            {
                Semaphore.Release();
            }
        }
    }
}
