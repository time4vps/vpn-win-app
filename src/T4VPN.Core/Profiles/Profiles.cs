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
using System.Linq;
using System.Threading.Tasks;
using T4VPN.Core.Profiles.Cached;
using T4VPN.Core.Profiles.Comparers;

namespace T4VPN.Core.Profiles
{
    public class Profiles : IProfileStorageAsync
    {
        private static readonly ProfileByIdEqualityComparer ProfileIdEqualityComparer =
            new ProfileByIdEqualityComparer();

        private readonly IProfileSource _predefinedProfiles;
        private readonly CachedProfiles _cachedProfiles;

        public Profiles(PredefinedProfiles predefinedProfiles, CachedProfiles cachedProfiles)
        {
            _predefinedProfiles = predefinedProfiles;
            _cachedProfiles = cachedProfiles;
        }

        public Task<IReadOnlyList<Profile>> GetAll()
        {
            var predefinedProfiles = _predefinedProfiles.GetAll();
            using (var cached = _cachedProfiles.ProfileData())
            {
                var cachedProfiles = cached.Local
                    .Union(cached.Sync, ProfileIdEqualityComparer)
                    .Union(cached.External, ProfileIdEqualityComparer)
                    .Where(p => p.Status != ProfileStatus.Deleted);

                IReadOnlyList<Profile> profiles = predefinedProfiles
                    .Concat(cachedProfiles)
                    .ToList();

                return Task.FromResult(profiles);
            }
        }

        public Task Create(Profile profile)
        {
            return AddOrReplaceLocal(profile, ProfileStatus.Created);
        }

        public Task Update(Profile profile)
        {
            return AddOrReplaceLocal(profile, ProfileStatus.Updated);
        }

        public Task Delete(Profile profile)
        {
            return AddOrReplaceLocal(profile, ProfileStatus.Deleted);
        }

        private async Task AddOrReplaceLocal(Profile profile, ProfileStatus status)
        {
            using (var cached = await _cachedProfiles.LockedProfileData())
            {
                var local = cached.Local.Get(profile);

                if (local?.Status == ProfileStatus.Created && status == ProfileStatus.Deleted)
                {
                    cached.Local.Remove(local);
                    return;
                }

                var mapped = Map(profile)
                    .WithStatus(status)
                    .WithSyncStatus(ProfileSyncStatus.InProgress)
                    .WithModifiedAt(DateTime.UtcNow);

                cached.Local.AddOrReplace(
                    mapped.WithStatusMergedFrom(local));
            }
        }

        private static Profile Map(Profile profile)
        {
            return profile.Clone();
        }
    }
}
