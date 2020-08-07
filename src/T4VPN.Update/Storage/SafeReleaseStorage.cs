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
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using T4VPN.Update.Helpers;
using T4VPN.Update.Releases;

namespace T4VPN.Update.Storage
{
    /// <summary>
    /// Wraps expected exceptions of <see cref="WebReleaseStorage"/> into <see cref="AppUpdateException"/>.
    /// </summary>
    internal class SafeReleaseStorage : IReleaseStorage
    {
        private readonly IReleaseStorage _storage;

        public SafeReleaseStorage(IReleaseStorage storage)
        {
            _storage = storage;
        }

        public async Task<IEnumerable<Release>> Releases()
        {
            try
            {
                return await _storage.Releases();
            }
            catch (JsonException e)
            {
                throw new AppUpdateException("Release history has unsupported format", e);
            }
            catch (Exception e) when (e.IsCommunicationException())
            {
                throw new AppUpdateException("Failed to download release history", e);
            }
        }
    }
}
