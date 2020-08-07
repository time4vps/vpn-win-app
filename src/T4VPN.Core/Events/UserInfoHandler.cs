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

using T4VPN.Core.Api;
using T4VPN.Core.Api.Contracts;
using T4VPN.Core.Settings;
using System.Net.Http;
using System.Threading.Tasks;

namespace T4VPN.Core.Events
{
    public class UserInfoHandler : IApiDataChangeAware
    {
        private readonly IApiClient _apiClient;
        private readonly IUserStorage _userStorage;

        public UserInfoHandler(IApiClient apiClient, IUserStorage userStorage)
        {
            _userStorage = userStorage;
            _apiClient = apiClient;
        }

        public async Task OnApiDataChanged(EventResponse eventResponse)
        {
            if (eventResponse.VpnSettings == null)
            {
                return;
            }

            try
            {
                var response = await _apiClient.GetVpnInfoResponse();
                if (response.Success)
                {
                    _userStorage.StoreVpnInfo(response.Value);
                }
            }
            catch (HttpRequestException)
            {
                //ignore
            }
        }
    }
}
