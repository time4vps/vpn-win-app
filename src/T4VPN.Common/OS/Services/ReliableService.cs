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

using System.Threading;
using System.Threading.Tasks;
using T4VPN.Common.Abstract;

namespace T4VPN.Common.OS.Services
{
    public class ReliableService : IService
    {
        private readonly IServiceRetryPolicy _retryPolicy;
        private readonly IService _origin;

        public ReliableService(IServiceRetryPolicy retryPolicy, IService origin)
        {
            _retryPolicy = retryPolicy;
            _origin = origin;
        }

        public string Name => _origin.Name;

        public bool Running() => _origin.Running();

        public Task<Result> StartAsync(CancellationToken cancellationToken)
        {
            return _retryPolicy.ExecuteAsync(ct => _origin.StartAsync(ct), cancellationToken);
        }

        public Task<Result> StopAsync(CancellationToken cancellationToken)
        {
            return _origin.StopAsync(cancellationToken);
        }
    }
}
