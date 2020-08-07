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
using System.Threading.Tasks;
using T4VPN.Common.Extensions;
using T4VPN.Update.Helpers;

namespace T4VPN.Update.Files.Downloadable
{
    /// <summary>
    /// Wraps expected exceptions of <see cref="DownloadableFile"/> into <see cref="AppUpdateException"/>.
    /// </summary>
    internal class SafeDownloadableFile : IDownloadableFile
    {
        private readonly IDownloadableFile _origin;

        public SafeDownloadableFile(IDownloadableFile origin)
        {
            _origin = origin;
        }

        public async Task Download(string url, string filename)
        {
            try
            {
                await _origin.Download(url, filename);
            }
            catch (Exception e) when (e.IsCommunicationException() || e.IsFileAccessException())
            {
                throw new AppUpdateException("Failed to download update", e);
            }
        }
    }
}
