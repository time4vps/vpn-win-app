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
using System.Threading.Tasks;
using T4VPN.Core.Api.Contracts;
using T4VPN.Core.Api.Data;
using UserLocation = T4VPN.Core.Api.Contracts.UserLocation;

namespace T4VPN.Core.Api
{
    public interface IApiClient
    {
        Task<ApiResponseResult<AuthResponse>> GetAuthResponse(AuthRequestData data);
        Task<ApiResponseResult<AuthInfo>> GetAuthInfoResponse(AuthInfoRequestData data);
        Task<ApiResponseResult<VpnInfoResponse>> GetVpnInfoResponse();
        Task<ApiResponseResult<BaseResponse>> GetLogoutResponse();
        Task<ApiResponseResult<EventResponse>> GetEventResponse(string lastId = default);
        Task<ApiResponseResult<ServerList>> GetServersAsync(string ip);
        Task<ApiResponseResult<UserLocation>> GetLocationDataAsync();
        Task<ApiResponseResult<BaseResponse>> ReportBugAsync(IEnumerable<KeyValuePair<string, string>> fields, IEnumerable<File> files);
        Task<ApiResponseResult<PricingPlans>> GetPricing(string currency, sbyte cycle);
        Task<ApiResponseResult<SessionsResponse>> GetSessions();
        Task<ApiResponseResult<ProfilesResponse>> GetProfiles();
        Task<ApiResponseResult<ProfileResponse>> CreateProfile(BaseProfile profile);
        Task<ApiResponseResult<ProfileResponse>> UpdateProfile(string id, BaseProfile profile);
        Task<ApiResponseResult<ProfileResponse>> DeleteProfile(string id);
        Task<ApiResponseResult<VpnConfig>> GetVpnConfig();
    }
}
