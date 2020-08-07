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
using System.Net.Http;
using System.Threading.Tasks;
using T4VPN.Common.Logging;
using T4VPN.Core.Abstract;
using T4VPN.Core.Api;
using T4VPN.Core.Api.Contracts;
using T4VPN.Core.Api.Data;
using T4VPN.Core.Settings;

namespace T4VPN.Core.Auth
{
    public class UserAuth
    {
        private readonly IApiClient _apiClient;
        private readonly ILogger _logger;
        private readonly IUserStorage _userStorage;
        private readonly ITokenStorage _tokenStorage;

        public UserAuth(
            IApiClient apiClient,
            ILogger logger,
            IUserStorage userStorage,
            ITokenStorage tokenStorage)
        {
            _tokenStorage = tokenStorage;
            _apiClient = apiClient;
            _logger = logger;
            _userStorage = userStorage;
        }

        public const int UserStatusVpnAccess = 1;
        public bool LoggedIn { get; private set; }

        public event EventHandler<EventArgs> UserLoggedOut;
        public event EventHandler<UserLoggedInEventArgs> UserLoggedIn;
        public event EventHandler<EventArgs> UserLoggingIn;

        public async Task<ApiResponseResult<VpnInfoResponse>> RefreshVpnInfo()
        {
            var vpnInfo = await _apiClient.GetVpnInfoResponse();
            if (vpnInfo.Success)
            {
                if (!vpnInfo.Value.Vpn.Status.Equals(UserStatusVpnAccess))
                {
                    return ApiResponseResult<VpnInfoResponse>.Fail(vpnInfo.StatusCode, "User has no VPN access.");
                }

                _userStorage.StoreVpnInfo(vpnInfo.Value);
            }

            return vpnInfo;
        }

        public async Task RefreshVpnInfo(Action<VpnInfoResponse> onSuccess)
        {
            try
            {
                var infoResult = await RefreshVpnInfo();
                if (infoResult.Success)
                {
                    onSuccess(infoResult.Value);
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.Error(ex.Message);
            }
        }

        public async Task<ApiResponseResult<AuthResponse>> LoginUserAsync(string username, string password)
        {
            _logger?.Info("Trying to login user");
            UserLoggingIn?.Invoke(this, EventArgs.Empty);

            var authResult = await AuthAsync(username, password);
            if (authResult.Failure)
            {
                return authResult;
            }

            var vpnInfo = await RefreshVpnInfo();
            if (vpnInfo.Success)
            {
                _userStorage.StoreVpnInfo(vpnInfo.Value);
                InvokeUserLoggedIn(false);
                return authResult;
            }

            return ApiResponseResult<AuthResponse>.Fail(vpnInfo.StatusCode, vpnInfo.Error);
        }

        public async Task<ApiResponseResult<AuthResponse>> AuthAsync(string username, string password)
        {           
            var authData = new AuthRequestData
            {
                TwoFactorCode = "",
                GrantType = "password",
                Username = username,
                Password = password
            };

            var response = await _apiClient.GetAuthResponse(authData);
            if (response.Success)
            {
                _userStorage.SaveUsername(username);
                _tokenStorage.AccessToken = response.Value.AccessToken;
                _tokenStorage.RefreshToken = response.Value.RefreshToken;
            }

            return response;
        }

        public void InvokeAutoLoginEvent()
        {
            InvokeUserLoggedIn(true);
        }

        private void InvokeUserLoggedIn(bool autoLogin)
        {
            LoggedIn = true;
            UserLoggedIn?.Invoke(this, new UserLoggedInEventArgs(autoLogin));
        }

        public void Logout()
        {
            if (!LoggedIn)
                return;

            LoggedIn = false;
            UserLoggedOut?.Invoke(this, EventArgs.Empty);

            SendLogoutRequest();

            _userStorage.ClearLogin();
        }

        private async void SendLogoutRequest()
        {
            try
            {
                await _apiClient.GetLogoutResponse();
            }
            catch (HttpRequestException e)
            {
                _logger.Error(e.Message);
            }
        }
    }
}
