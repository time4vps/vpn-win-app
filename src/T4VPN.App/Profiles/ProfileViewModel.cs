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

using T4VPN.ConnectionInfo;
using T4VPN.Core.MVVM;
using T4VPN.Core.Profiles;
using T4VPN.Core.Servers;
using T4VPN.Core.Servers.Models;
using T4VPN.Core.Servers.Name;

namespace T4VPN.Profiles
{
    public class ProfileViewModel : ViewModel
    {
        private readonly ProfileSyncStatus _originSyncStatus;

        public ProfileViewModel(Profile profile)
        {
            Id = profile.Id;
            IsPredefined = profile.IsPredefined;
            Name = profile.Name;
            Protocol = profile.Protocol;
            Color = profile.ColorCode;
            SecureCore = profile.Features.IsSecureCore();
            Type = profile.ProfileType;

            if (profile.Server != null)
            {
                ConnectionInfoViewModel = new ConnectionInfoViewModel(profile.Server);
            }

            _syncStatus = profile.SyncStatus;
            _originSyncStatus = profile.SyncStatus;
        }

        public bool ShowBottomBorder { get; set; } = true;
        public bool UpgradeRequired { get; set; }

        public ConnectionInfoViewModel ConnectionInfoViewModel { get; }

        public string Id { get; }
        public bool IsPredefined { get; }
        public string Name { get; }
        public Protocol Protocol { get; }
        public string Color { get; }
        public bool SecureCore { get; set; }
        public bool Connected { get; set; }
        public Server Server { get; set; }
        public IName ConnectionName { get; set; }
        public ProfileType Type { get; set; }

        private ProfileSyncStatus _syncStatus;
        public ProfileSyncStatus SyncStatus
        {
            get => _syncStatus;
            private set => Set(ref _syncStatus, value);
        }

        public void OnProfileSyncStatusChanged(ProfileSyncStatus status)
        {
            SyncStatus = status == ProfileSyncStatus.Failed && _originSyncStatus == ProfileSyncStatus.InProgress
                ? ProfileSyncStatus.Failed
                : _originSyncStatus;
        }
    }
}
