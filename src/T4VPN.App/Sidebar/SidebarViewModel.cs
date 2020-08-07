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

using Caliburn.Micro;
using GalaSoft.MvvmLight.Command;
using T4VPN.Core.Auth;
using T4VPN.Core.Settings;
using T4VPN.Onboarding;
using T4VPN.Sidebar.Trial;
using T4VPN.Trial;
using System.Threading.Tasks;
using System.Windows.Input;
using T4VPN.Core.User;
using T4VPN.FlashNotifications;

namespace T4VPN.Sidebar
{
    internal class SidebarViewModel :
        SidebarBaseViewModel,
        IOnboardingStepAware,
        ILogoutAware,
        ILoggedInAware,
        ITrialStateAware
    {
        public CountriesViewModel Countries { get; }
        public SidebarProfilesViewModel Profiles { get; }
        public ConnectionStatusViewModel ConnectionStatus { get; }
        public FlashNotificationViewModel FlashNotification { get; }
        private readonly IAppSettings _appSettings;

        private const int CountriesTab = 0;
        private const int ProfilesTab = 1;

        private Screen _tab;
        private bool _showTrialView;
        private bool _showSecondOnboardingStep;
        private bool _showThirdOnboardingStep;

        public SidebarViewModel(
            IAppSettings appSettings,
            SidebarManager sidebarManager,
            SidebarProfilesViewModel sidebarProfilesViewModel,
            TrialViewModel trialViewModel,
            ConnectionStatusViewModel connectionStatusViewModel,
            CountriesViewModel countriesViewModel,
            FlashNotificationViewModel flashNotificationsViewModel) : base(appSettings, sidebarManager)
        {
            _appSettings = appSettings;
            CountriesTabCommand = new RelayCommand(OpenCountriesTabAction);
            ProfilesTabCommand = new RelayCommand(OpenProfilesTabAction);

            Tab = countriesViewModel;
            TrialViewModel = trialViewModel;
            Countries = countriesViewModel;
            Profiles = sidebarProfilesViewModel;
            ConnectionStatus = connectionStatusViewModel;
            FlashNotification = flashNotificationsViewModel;
        }

        public ICommand CountriesTabCommand { get; }
        public ICommand ProfilesTabCommand { get; }

        public TrialViewModel TrialViewModel { get; set; }

        public Screen Tab
        {
            get => _tab;
            set => Set(ref _tab, value);
        }

        public bool ShowTrialView
        {
            get => _showTrialView;
            set => Set(ref _showTrialView, value);
        }

        public bool ShowSecondOnboardingStep
        {
            get => _showSecondOnboardingStep;
            set => Set(ref _showSecondOnboardingStep, value);
        }

        public bool ShowThirdOnboardingStep
        {
            get => _showThirdOnboardingStep;
            set => Set(ref _showThirdOnboardingStep, value);
        }

        public void OnStepChanged(int step)
        {
            ShowSecondOnboardingStep = step == 2;
            ShowThirdOnboardingStep = step == 3;
            if (step == 5)
            {
                OpenCountriesTabAction();
            }
        }

        public void OnUserLoggedOut()
        {
            ShowTrialView = false;
        }

        private void OpenProfilesTabAction()
        {
            Tab = Profiles;
            _appSettings.SidebarTab = ProfilesTab;
        }

        private void OpenCountriesTabAction()
        {
            Tab = Countries;
            _appSettings.SidebarTab = CountriesTab;
        }

        public Task OnTrialStateChangedAsync(PlanStatus status)
        {
            ShowTrialView = status.Equals(PlanStatus.Free) || status.Equals(PlanStatus.TrialStarted);

            return Task.CompletedTask;
        }

        public void OnUserLoggedIn()
        {
            if (_appSettings.SidebarTab == ProfilesTab)
                OpenProfilesTabAction();
        }
    }
}
