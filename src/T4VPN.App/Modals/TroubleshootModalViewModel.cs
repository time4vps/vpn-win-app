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

using System.ComponentModel;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using T4VPN.Config.Url;
using T4VPN.Core.Settings;

namespace T4VPN.Modals
{
    public class TroubleshootModalViewModel : BaseModalViewModel, ISettingsAware
    {
        private readonly IActiveUrls _urls;
        private readonly IAppSettings _appSettings;

        public TroubleshootModalViewModel(IActiveUrls urls, IAppSettings appSettings)
        {
            _urls = urls;
            _appSettings = appSettings;

            Time4VPSStatusCommand = new RelayCommand(OpenTime4VPSStatusPage);
            Time4VPSTwitterCommand = new RelayCommand(OpenTime4VPSTwitterPage);
            TorCommand = new RelayCommand(OpenTorPage);
            SupportFormCommand = new RelayCommand(OpenSupportFormPage);
            AlternativeRoutingCommand = new RelayCommand(OpenAlternativeRoutingPage);
        }

        public ICommand Time4VPSStatusCommand { get; }
        public ICommand Time4VPSTwitterCommand { get; }
        public ICommand TorCommand { get; }
        public ICommand SupportFormCommand { get; }
        public ICommand AlternativeRoutingCommand { get; }

        public bool DoHEnabled
        {
            get => _appSettings.DoHEnabled;
            set => _appSettings.DoHEnabled = value;
        }

        public override void OnAppSettingsChanged(PropertyChangedEventArgs e)
        {
            base.OnAppSettingsChanged(e);

            if (e.PropertyName == nameof(IAppSettings.DoHEnabled))
            {
                DoHEnabled = _appSettings.DoHEnabled;
            }
        }

        private void OpenTime4VPSStatusPage()
        {
            _urls.Time4VPSStatusUrl.Open();
        }

        private void OpenTime4VPSTwitterPage()
        {
            _urls.Time4VPSTwitterUrl.Open();
        }

        private void OpenTorPage()
        {
            _urls.TorBrowserUrl.Open();
        }

        private void OpenSupportFormPage()
        {
            _urls.SupportFormUrl.Open();
        }

        private void OpenAlternativeRoutingPage()
        {
            _urls.AlternativeRoutingUrl.Open();
        }
    }
}
