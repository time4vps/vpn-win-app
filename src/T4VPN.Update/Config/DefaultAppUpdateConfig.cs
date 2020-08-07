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

using T4VPN.Common.OS.Net.Http;
using System;

namespace T4VPN.Update.Config
{
    /// <summary>
    /// The simple DTO object for providing configuration data to Update module.
    /// </summary>
    public class DefaultAppUpdateConfig : IAppUpdateConfig
    {
        public IHttpClient HttpClient { get; set; }
        public Uri FeedUri { get; set; }
        public Version CurrentVersion { get; set; }
        public string UpdatesPath { get; set; }
        public string EarlyAccessCategoryName { get; set; }
        public TimeSpan MinProgressDuration { get; set; }
    }
}
