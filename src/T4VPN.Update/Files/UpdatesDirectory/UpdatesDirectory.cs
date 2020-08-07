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
using System.Diagnostics;
using System.IO;
using T4VPN.Common.Extensions;
using T4VPN.Common.Helpers;

namespace T4VPN.Update.Files.UpdatesDirectory
{
    /// <summary>
    /// Represents directory of downloaded updates, performs cleanup of outdated downloads.
    /// </summary>
    internal class UpdatesDirectory : IUpdatesDirectory
    {
        private readonly string _path;
        private readonly Version _currentVersion;

        public UpdatesDirectory(string path, Version currentVersion)
        {
            Ensure.NotEmpty(path, nameof(path));

            _path = path;
            _currentVersion = currentVersion;
        }

        public string Path
        {
            get
            {
                Directory.CreateDirectory(_path);
                return _path;
            }
        }

        public void Cleanup()
        {
            var dirs = Directory.GetDirectories(_path);
            foreach (var dir in dirs)
            {
                Directory.Delete(dir, true);
            }
            DeleteOldFiles(_path, _currentVersion);
        }

        private static void DeleteOldFiles(string dir, Version currentVersion)
        {
            var files = Directory.GetFiles(dir, "*", SearchOption.TopDirectoryOnly);
            foreach (var file in files)
            {
                if (!IsExeFile(file) || FileVersion(file) <= currentVersion)
                    File.Delete(file);
            }
        }

        private static bool IsExeFile(string path) =>
            string.Equals(System.IO.Path.GetExtension(path), ".exe", StringComparison.OrdinalIgnoreCase);

        private static Version FileVersion(string path)
        {
            var info = FileVersionInfo.GetVersionInfo(path);
            Version.TryParse(info.FileVersion, out var version);
            return version != null 
                ? version.Normalized() 
                : new Version(0, 0, 0);
        }
    }
}
