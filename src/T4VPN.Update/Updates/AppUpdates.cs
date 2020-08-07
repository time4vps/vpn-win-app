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

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using T4VPN.Common.Helpers;
using T4VPN.Update.Config;
using T4VPN.Update.Files;
using T4VPN.Update.Files.Downloadable;
using T4VPN.Update.Files.Launchable;
using T4VPN.Update.Files.UpdatesDirectory;
using T4VPN.Update.Files.Validatable;
using T4VPN.Update.Releases;
using T4VPN.Update.Storage;

namespace T4VPN.Update.Updates
{
    /// <summary>
    /// Performs app update related operations.
    /// Provides release history, checks for update, downloads, validates and starts update.
    /// </summary>
    internal class AppUpdates : IAppUpdates
    {
        private readonly IReleaseStorage _releaseStorage;
        private readonly IUpdatesDirectory _updatesDirectory;
        private readonly FileLocation _fileLocation;
        private readonly IDownloadableFile _downloadable;
        private readonly IValidatableFile _validatable;
        private readonly ILaunchableFile _launchable;

        public AppUpdates(IAppUpdateConfig config, ILaunchableFile launchableFile)
        {
            Ensure.NotNull(config, nameof(config));

            config.Validate();

            _releaseStorage =
                new OrderedReleaseStorage(
                    new SafeReleaseStorage(
                        new WebReleaseStorage(config)));

            _updatesDirectory = 
                new SafeUpdatesDirectory(
                    new UpdatesDirectory(config.UpdatesPath, config.CurrentVersion)
                );

            _fileLocation = new FileLocation(_updatesDirectory.Path);

            _downloadable = 
                new SafeDownloadableFile( 
                    new DownloadableFile(config.HttpClient));

            _validatable = 
                new SafeValidatableFile(
                    new CachingValidatableFile(
                        new ValidatableFile()));

            _launchable = new SafeLaunchableFile(launchableFile);
        }

        public void Cleanup()
        {
            _updatesDirectory.Cleanup();
        }

        internal async Task<IReadOnlyList<Release>> ReleaseHistory(bool earlyAccess)
        {
            var releases = await _releaseStorage.Releases();
            return releases.ToList();
        }

        internal async Task Download(Release release)
        {
            await _downloadable.Download(release.File.Url, FilePath(release));
        }

        internal async Task<bool> Valid(Release release)
        {
            return await _validatable.Valid(FilePath(release), release.File.Sha512CheckSum);
        }

        internal Task StartUpdate(Release release)
        {
            _launchable.Launch(FilePath(release), release.File.Arguments);
            return Task.CompletedTask;
        }

        internal string FilePath(Release release) => _fileLocation.Path(release.File.Url);
    }
}
