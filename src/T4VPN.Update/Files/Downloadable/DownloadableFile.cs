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

using System.IO;
using System.Threading.Tasks;
using T4VPN.Common.OS.Net.Http;

namespace T4VPN.Update.Files.Downloadable
{
    /// <summary>
    /// Downloads file from internet.
    /// </summary>
    internal class DownloadableFile : IDownloadableFile
    {
        private const int FileBufferSize = 16768;

        private readonly IHttpClient _client;

        public DownloadableFile(IHttpClient client)
        {
            _client = client;
        }

        public async Task Download(string url, string filename)
        {
            using (var response = await _client.GetAsync(url))
            {
                if (!response.IsSuccessStatusCode)
                    throw new AppUpdateException("Failed to download file");

                using (var contentStream = await response.Content.ReadAsStreamAsync())
                using (var fileStream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None, FileBufferSize, true))
                    await contentStream.CopyToAsync(fileStream);
            }
        }
    }
}
