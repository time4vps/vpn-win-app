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
using T4VPN.Core.Modals;
using T4VPN.Core.Profiles;
using T4VPN.Core.Servers;
using T4VPN.Core.Servers.Specs;
using T4VPN.Core.Settings;
using T4VPN.Core.User;
using T4VPN.Modals.Upsell;
using T4VPN.Profiles.Servers;

namespace T4VPN.Profiles.Form
{
    public abstract class BaseCountryServerProfileFormViewModel : AbstractForm, IVpnPlanAware
    {
        private CountryViewModel _selectedCountry;
        private List<CountryViewModel> _countries;
        private readonly IModals _modals;
        private bool _unsavedChanges;

        protected BaseCountryServerProfileFormViewModel(
            Common.Configuration.Config appConfig,
            ColorProvider colorProvider,
            IUserStorage userStorage,
            ProfileManager profileManager,
            IDialogs dialogs,
            IModals modals,
            ServerManager serverManager) : base(appConfig, colorProvider, userStorage, profileManager, dialogs, modals, serverManager)
        {
            _modals = modals;
        }

        public List<CountryViewModel> Countries
        {
            get => _countries;
            set => Set(ref _countries, value);
        }

        public CountryViewModel SelectedCountry
        {
            get => _selectedCountry;
            set
            {
                if (value?.CountryCode != _selectedCountry?.CountryCode)
                {
                    if (ShowUpgradeModal(value))
                    {
                        _modals.Show<NoServerInCountryDueTierUpsellModalViewModel>();
                    }

                    if (EditMode && _selectedCountry != null)
                    {
                        _unsavedChanges = true;
                    }

                    _selectedCountry = value;
                    NotifyOfPropertyChange();
                }

                LoadServers();
                SelectedServer = GetSelectedServer();
            }
        }

        public Task OnVpnPlanChangedAsync(string plan)
        {
            LoadCountries();
            return Task.CompletedTask;
        }

        public override bool HasUnsavedChanges()
        {
            return _unsavedChanges || base.HasUnsavedChanges();
        }

        public override void Load()
        {
            base.Load();
            if (EditMode)
                return;

            LoadCountries();
            LoadServers();
        }

        public override void LoadProfile(Profile profile)
        {
            LoadCountries();
            SelectedCountry = Countries.FirstOrDefault(c => c.CountryCode.Equals(profile.CountryCode));
            base.LoadProfile(profile);
        }

        public override void Clear()
        {
            base.Clear();
            SelectedCountry = null;
            Countries = null;
            _unsavedChanges = false;
        }

        protected override Profile GetProfile()
        {
            var profile = base.GetProfile();
            profile.CountryCode = SelectedCountry?.CountryCode;
            profile.ServerId = SelectedServer?.Id;
            return profile;
        }

        protected override async Task<Error> GetFormError()
        {
            var error = await base.GetFormError();
            if (error != Error.None)
            {
                if (error == Error.EmptyServer && SelectedCountry == null)
                {
                    return Error.EmptyCountry;
                }

                return error;
            }

            if (SelectedCountry == null)
            {
                return Error.EmptyCountry;
            }

            return Error.None;
        }

        protected virtual List<IServerViewModel> GetServersByCountry(string countryCode)
        {
            var spec = new ServerByFeatures(GetFeatures()) &&
                       new ExitCountryServer(countryCode);
            var countryServers = ServerManager.GetServers(spec);

            var servers = GetPredefinedServerViewModels()
                .Union(GetServerViewModels(countryServers))
                .ToList();

            return servers;
        }

        private IEnumerable<string> GetCountries()
        {
            return ServerManager.GetServers(new ServerByFeatures(GetFeatures()))
                .Select(s => s.ExitCountry)
                .Distinct();
        }

        private bool IsUpgradeRequiredForCountry(string countryCode)
        {
            var spec = new ServerByFeatures(GetFeatures()) &&
                       new ExitCountryServer(countryCode) &&
                       new MaxTierServer(UserStorage.User().MaxTier);

            return !ServerManager.GetServers(spec).Any();
        }

        private void LoadCountries()
        {
            Countries = GetCountries()
                .OrderBy(T4VPN.Servers.Countries.GetName)
                .Select(c => new CountryViewModel(c, IsUpgradeRequiredForCountry(c)))
                .ToList();
        }

        private void LoadServers()
        {
            Servers = GetServersByCountry(SelectedCountry?.CountryCode);
        }

        private bool ShowUpgradeModal(CountryViewModel value)
        {
            if (value != null && value.UpgradeRequired)
            {
                if (EditMode)
                {
                    if (SelectedCountry != null && !SelectedCountry.UpgradeRequired)
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }

            return false;
        }

        private IServerViewModel GetSelectedServer()
        {
            if (EditMode)
            {
                return _unsavedChanges
                    ? Servers?.FirstOrDefault()
                    : Servers?.FirstOrDefault(s => s.Name == SelectedServer?.Name);
            }

            return Servers?.FirstOrDefault();
        }
    }
}
