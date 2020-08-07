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
using System.Net.Http;
using System.Threading.Tasks;
using T4VPN.Core.Api;
using T4VPN.Core.Api.Contracts;
using T4VPN.Core.Settings;

namespace T4VPN.Core.Events
{
    public class EventClient
    {
        private readonly IApiClient _apiClient;
        private readonly IAppSettings _appSettings;

        public event EventHandler<EventResponse> ApiDataChanged;

        public EventClient(IApiClient apiClient, IAppSettings appSettings)
        {
            _appSettings = appSettings;
            _apiClient = apiClient;
        }

        public async Task GetEvents()
        {
            // TODO: not implemented
            
            return;

            try
            {
                var response = await _apiClient.GetEventResponse(_appSettings.LastEventId);
                if (response.Success && _appSettings.LastEventId != response.Value.EventId)
                {
                    _appSettings.LastEventId = response.Value.EventId;
                    ApiDataChanged?.Invoke(this, response.Value);
                }
            }
            catch (HttpRequestException)
            {
                //ignore
            }
        }

        public async Task StoreLatestEvent()
        {
            // TODO: not implemented

            return;

            try
            {
                var response = await _apiClient.GetEventResponse();
                if (response.Success)
                {
                    _appSettings.LastEventId = response.Value.EventId;
                }
            }
            catch (HttpRequestException)
            {
                //ignore
            }
        }
    }
}
