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
using System.ComponentModel;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using T4VPN.Core.MVVM;
using T4VPN.Core.Profiles;
using T4VPN.Core.Settings;
using T4VPN.Resources;

namespace T4VPN.Profiles
{
    public class ProfileSyncViewModel : ViewModel, IProfileSyncStatusAware, ISettingsAware
    {
        private readonly ISyncProfileStorage _syncProfiles;
        private DateTime _changesSyncedAt = DateTime.Now;
        private string _errorMessage = string.Empty;

        public ProfileSyncViewModel(ISyncProfileStorage syncProfiles)
        {
            _syncProfiles = syncProfiles;

            SyncCommand = new RelayCommand(SyncAction);
        }

        public ICommand SyncCommand { get; }

        private ProfileSyncStatus _syncStatus;
        public ProfileSyncStatus SyncStatus
        {
            get => _syncStatus;
            set => Set(ref _syncStatus, value);
        }

        private string _syncStatusMessage;
        public string SyncStatusMessage
        {
            get => _syncStatusMessage;
            set => Set(ref _syncStatusMessage, value);
        }

        public void OnProfileSyncStatusChanged(ProfileSyncStatus status, string errorMessage, DateTime changesSyncedAt)
        {
            SyncStatus = status;
            _errorMessage = errorMessage;
            _changesSyncedAt = changesSyncedAt;

            switch (status)
            {
                case ProfileSyncStatus.Succeeded:
                    if (changesSyncedAt == DateTime.MinValue)
                        SyncStatusMessage = StringResources.Get("ProfileSyncStatus_val_Succeeded_Info_NoData");
                    else
                    {
                        var (number, units) = Elapsed(changesSyncedAt);
                        SyncStatusMessage = StringResources.Format("ProfileSyncStatus_val_Succeeded_Info", number, units);
                    }
                    break;
                case ProfileSyncStatus.InProgress:
                    SyncStatusMessage = StringResources.Get("ProfileSyncStatus_val_InProgress_Info");
                    break;
                case ProfileSyncStatus.Failed:
                    SyncStatusMessage = StringResources.Get("ProfileSyncStatus_val_Failed_Info");
                    break;
                case ProfileSyncStatus.Overridden:
                    throw new NotSupportedException();
                default:
                    SyncStatusMessage = string.Empty;
                    break;
            }
        }

        public void OnAppSettingsChanged(PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(IAppSettings.Language)) && SyncStatus == ProfileSyncStatus.Succeeded)
            {
                OnProfileSyncStatusChanged(SyncStatus, _errorMessage, _changesSyncedAt);
            }
        }

        private (int Number, string Units) Elapsed(DateTime changesSyncedAt)
        {
            var elapsed = DateTime.UtcNow - changesSyncedAt;
            if (elapsed < TimeSpan.Zero)
                elapsed = TimeSpan.Zero;

            return elapsed.Days > 0 ? (elapsed.Days, StringResources.GetPlural("TimeUnit_val_Day", elapsed.Days)) :
                elapsed.Hours > 0 ? (elapsed.Hours, StringResources.GetPlural("TimeUnit_val_Hour", elapsed.Hours)) :
                (elapsed.Minutes, StringResources.GetPlural("TimeUnit_val_Minute", elapsed.Minutes));
        }

        private void SyncAction()
        {
            _syncProfiles.Sync();
        }
    }
}
