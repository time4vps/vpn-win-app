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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using T4VPN.Common.Configuration;
using T4VPN.Common.Extensions;
using T4VPN.Common.Storage;
using T4VPN.Common.Threading;
using T4VPN.Core.Api.Contracts;
using T4VPN.Core.Auth;
using T4VPN.Core.User;

namespace T4VPN.Core.Servers
{
    public class ServerUpdater : IServerUpdater, ILoggedInAware, ILogoutAware, IUserLocationAware
    {
        private readonly ISchedulerTimer _timer;
        private readonly ServerManager _serverManager;
        private readonly ApiServers _apiServers;
        private readonly ICollectionStorage<LogicalServerContract> _serverCache;

        private readonly SingleAction _updateAction;

        private bool _firstTime = true;

        public ServerUpdater(
            IScheduler scheduler,
            Config appConfig,
            ServerManager serverManager,
            ApiServers apiServers,
            ICollectionStorage<LogicalServerContract> serverCache)
        {
            _serverManager = serverManager;
            _apiServers = apiServers;
            _serverCache = serverCache;

            _timer = scheduler.Timer();
            _timer.Interval = appConfig.ServerUpdateInterval.RandomizedWithDeviation(0.2);
            _timer.Tick += Timer_OnTick;

            _updateAction = new SingleAction(UpdateServers);
        }

        public event EventHandler ServersUpdated;

        public void OnUserLoggedIn()
        {
            _timer.Start();
        }

        public void OnUserLoggedOut()
        {
            _timer.Stop();
            _firstTime = true;
        }

        public async Task Update()
        {
            await _updateAction.Run();
        }

        public Task OnUserLocationChanged(UserLocationEventArgs e)
        {
            if (e.State == UserLocationState.Success)
            {
                _ = Update();
            }

            return Task.CompletedTask;
        }

        private async Task UpdateServers()
        {
            var servers = await GetServers();

            if (!servers.Any())
                return;

            _serverManager.Load(servers);
            InvokeServersUpdated();
            _serverCache.SetAll(servers);
        }

        private async Task<IReadOnlyCollection<LogicalServerContract>> GetServers()
        {
            if (!_firstTime)
            {
                return await _apiServers.GetAsync();
            }

            _firstTime = false;

            if (_serverManager.Empty())
            {
                return await _apiServers.GetAsync();
            }

            // First time after start or logoff server update is scheduled without waiting for the result
            ScheduleUpdate();

            return new List<LogicalServerContract>(0);
        }

        private void ScheduleUpdate()
        {
            // Schedule servers update from API without waiting for the result
            _updateAction.Task.ContinueWith(t => Update());
        }

        private void Timer_OnTick(object sender, EventArgs eventArgs)
        {
            _updateAction.Run();
        }

        private void InvokeServersUpdated()
        {
            ServersUpdated?.Invoke(this, EventArgs.Empty);
        }
    }
}
