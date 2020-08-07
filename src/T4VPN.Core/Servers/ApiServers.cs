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

using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using T4VPN.Common.Extensions;
using T4VPN.Common.Logging;
using T4VPN.Core.Api;
using T4VPN.Core.Api.Contracts;
using T4VPN.Core.User;

namespace T4VPN.Core.Servers
{
    public class ApiServers
    {
        private readonly ILogger _logger;
        private readonly IApiClient _apiClient;
        private readonly TruncatedLocation _location;

        public ApiServers(
            ILogger logger,
            IApiClient apiClient,
            TruncatedLocation location)
        {
            _logger = logger;
            _apiClient = apiClient;
            _location = location;
        }

        public async Task<IReadOnlyCollection<LogicalServerContract>> GetAsync()
        {
            try
            {
                var response = await _apiClient.GetServersAsync(_location.Ip());
                if (response.Success)
                {
                    return response.Value.Servers;
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.Error("API: Get servers failed: " + ex.CombinedMessage());
            }

            return new LogicalServerContract[0];
        }
    }
}
