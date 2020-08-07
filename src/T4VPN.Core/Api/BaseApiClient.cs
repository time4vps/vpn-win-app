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

using Newtonsoft.Json;
using T4VPN.Common.OS.Net.Http;
using T4VPN.Core.Abstract;
using T4VPN.Core.Api.Contracts;
using System.Net;
using System.Net.Http;
using System.Text;

namespace T4VPN.Core.Api
{
    public class BaseApiClient
    {
        private readonly IApiAppVersion _appVersion;
        protected readonly ITokenStorage TokenStorage;
        private readonly string _apiVersion;
        private readonly string _locale;

        public BaseApiClient(
            IApiAppVersion appVersion,
            ITokenStorage tokenStorage,
            string apiVersion,
            string locale)
        {
            _apiVersion = apiVersion;
            TokenStorage = tokenStorage;
            _appVersion = appVersion;
            _locale = locale;
        }

        protected StringContent GetJsonContent(object data)
        {
            var json = JsonConvert.SerializeObject(data);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        protected ApiResponseResult<T> ApiResponseResult<T>(string body, HttpStatusCode code) where T : BaseResponse
        {
            try
            {
                var response = JsonConvert.DeserializeObject<T>(body);
                if (response == null)
                {
                    throw new HttpRequestException(string.Empty);
                }

                return response.Code == ResponseCodes.OkResponse
                    ? Api.ApiResponseResult<T>.Ok(response)
                    : Api.ApiResponseResult<T>.Fail(response, code, response.Error);
            }
            catch (JsonException)
            {
                throw new HttpRequestException(code.Description());
            }
        }

        protected HttpRequestMessage GetRequest(HttpMethod method, string requestUri)
        {
            var request = new HttpRequestMessage(method, requestUri);
            request.Headers.Add("x-t4v-apiversion", _apiVersion);
            request.Headers.Add("x-t4v-appversion", _appVersion.Value());
            request.Headers.Add("x-t4v-locale", _locale);
            request.Headers.Add("User-Agent", _appVersion.UserAgent());

            return request;
        }

        protected HttpRequestMessage GetAuthorizedRequest(HttpMethod method, string requestUri)
        {
            var request = GetRequest(method, requestUri);
            request.Headers.Add("Authorization", $"Bearer {TokenStorage.AccessToken}");

            return request;
        }
    }
}
