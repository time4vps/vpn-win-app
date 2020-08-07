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
using T4VPN.Common.Configuration;
using T4VPN.Core.Settings;

namespace T4VPN.Core.Api
{
    public class ApiAppVersion : IApiAppVersion
    {
        private readonly IAppSettings _appSettings;
        private readonly Config _appConfig;

        public ApiAppVersion(IAppSettings appSettings, Config appConfig)
        {
            _appConfig = appConfig;
            _appSettings = appSettings;
        }

        public string Value()
        {
            var version = $"{_appConfig.ApiClientId}_{GetVersion()}";
            return _appSettings.EarlyAccess ? $"{version}-early" : version;
        }

        public string UserAgent()
        {
            return $"{_appConfig.UserAgent}/{GetVersion()} ({Environment.OSVersion})";
        }

        private string GetVersion()
        {
            return _appSettings.EarlyAccess ? $"{_appConfig.AppVersion}-early" : _appConfig.AppVersion;
        }
    }
}