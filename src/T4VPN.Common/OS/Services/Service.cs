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
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using T4VPN.Common.Abstract;
using T4VPN.Common.Extensions;
using T4VPN.Common.Helpers;

namespace T4VPN.Common.OS.Services
{
    public abstract class Service : IService
    {
        private static readonly TimeSpan WaitForServiceStatusDuration = TimeSpan.FromSeconds(30);

        protected Service(string serviceName)
        {
            Ensure.NotEmpty(serviceName, nameof(serviceName));

            Name = serviceName;
        }

        public string Name { get; }

        public bool Running()
        {
            return GetServices()
                .Where(s => s.Status == ServiceControllerStatus.Running)
                .Select(s => s.ServiceName)
                .ContainsIgnoringCase(Name);
        }

        public Task<Result> StartAsync(CancellationToken cancellationToken)
        {
            return ServiceControllerResult(async sc =>
            {
                sc.Start();
                await sc.WaitForStatusAsync(ServiceControllerStatus.Running, WaitForServiceStatusDuration, cancellationToken);
            });
        }

        public Task<Result> StopAsync(CancellationToken cancellationToken)
        {
            return ServiceControllerResult(sc =>
            {
                sc.Stop();
                return Task.CompletedTask;
            });
        }

        protected abstract ServiceController[] GetServices();

        private async Task<Result> ServiceControllerResult(Func<ServiceController, Task> action)
        {
            using var sc = new ServiceController(Name);
            await action(sc);
            return Result.Ok();
        }
    }
}
