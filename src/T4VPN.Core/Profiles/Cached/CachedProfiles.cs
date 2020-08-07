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

using System.Threading.Tasks;
using T4VPN.Common.Threading;
using T4VPN.Core.Settings;

namespace T4VPN.Core.Profiles.Cached
{
    public class CachedProfiles
    {
        private readonly SerialTaskQueue _accessQueue = new SerialTaskQueue();
        private readonly IAppSettings _appSettings;

        public CachedProfiles(IAppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public async Task<CachedProfileData> LockedProfileData()
        {
            return new CachedProfileData(_appSettings, await _accessQueue.Lock());
        }

        public CachedProfileData ProfileData()
        {
            return new CachedProfileData(_appSettings);
        }
    }
}
