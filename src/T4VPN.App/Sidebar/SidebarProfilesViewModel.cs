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
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Caliburn.Micro;
using GalaSoft.MvvmLight.CommandWpf;
using T4VPN.Core.Modals;
using T4VPN.Core.Profiles;
using T4VPN.Core.Servers;
using T4VPN.Core.Service.Vpn;
using T4VPN.Core.Settings;
using T4VPN.Profiles;

namespace T4VPN.Sidebar
{
    internal class SidebarProfilesViewModel : Screen, ISettingsAware, IServersAware
    {
        private readonly ProfileManager _profileManager;
        private readonly ProfileViewModelFactory _profileHelper;
        private readonly IVpnManager _vpnManager;
        private readonly IModals _modals;

        public SidebarProfilesViewModel(
            ProfileManager profileManager,
            ProfileViewModelFactory profileHelper,
            IVpnManager vpnManager,
            IModals modals)
        {
            _modals = modals;
            _profileManager = profileManager;
            _profileHelper = profileHelper;
            _vpnManager = vpnManager;

            CreateProfileCommand = new RelayCommand(CreateProfileAction);
            ManageProfilesCommand = new RelayCommand(ManageProfilesAction);
            ConnectCommand = new RelayCommand<ProfileViewModel>(ConnectAction);
        }

        public ICommand ConnectCommand { get; set; }
        public ICommand CreateProfileCommand { get; set; }
        public ICommand ManageProfilesCommand { get; set; }

        private IReadOnlyList<ProfileViewModel> _profiles;
        public IReadOnlyList<ProfileViewModel> Profiles
        {
            get => _profiles;
            set => Set(ref _profiles, value);
        }

        public async void Load()
        {
            await LoadProfiles();
        }

        public async void OnAppSettingsChanged(PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(IAppSettings.Profiles)) ||
                e.PropertyName.Equals(nameof(IAppSettings.SecureCore)) ||
                e.PropertyName.Equals(nameof(IAppSettings.Language)))
            {
                await LoadProfiles();
            }
        }

        public void OnServersUpdated()
        {
            Load();
        }

        private async void ConnectAction(ProfileViewModel viewModel)
        {
            if (viewModel == null) return;
            var profile = await _profileManager.GetProfileById(viewModel.Id);
            if (profile == null) return;

            await _vpnManager.Connect(profile);
        }

        private async Task LoadProfiles()
        {
            Profiles = (await _profileHelper.GetProfiles())
                .OrderByDescending(p => p.IsPredefined)
                .ThenBy(p => p.Name)
                .ToList();
        }

        private void CreateProfileAction()
        {
            _modals.Show<ProfileFormModalViewModel>();
        }

        private void ManageProfilesAction()
        {
            _modals.Show<ProfileListModalViewModel>();
        }
    }
}
