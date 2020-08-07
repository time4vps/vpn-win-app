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

using T4VPN.Core.Modals;
using T4VPN.Core.Profiles;
using T4VPN.Core.Servers;
using T4VPN.Core.Servers.Models;
using T4VPN.Core.Servers.Specs;
using T4VPN.Core.Settings;
using System.Collections.Generic;
using System.Linq;

namespace T4VPN.Profiles.Form
{
    public abstract class ServerProfileFormViewModel : AbstractForm
    {
        protected ServerProfileFormViewModel(
            Common.Configuration.Config appConfig,
            ColorProvider colorProvider,
            IUserStorage userStorage,
            ProfileManager profileManager,
            IDialogs dialogs,
            IModals modals,
            ServerManager serverManager) : base(appConfig, colorProvider, userStorage, profileManager, dialogs, modals, serverManager)
        {
        }

        public override void Load()
        {
            base.Load();
            if (!EditMode)
            {
                LoadServers();
                SelectedServer = Servers.FirstOrDefault();
            }
        }

        public override void LoadProfile(Profile profile)
        {
            LoadServers();
            base.LoadProfile(profile);
        }

        public override void Clear()
        {
            base.Clear();
            SelectedServer = null;
        }

        protected override Profile GetProfile()
        {
            var profile = base.GetProfile();
            profile.ServerId = SelectedServer?.Id;
            return profile;
        }

        protected virtual List<Server> GetServerList()
        {
            return ServerManager.GetServers(new ServerByFeatures(GetFeatures())).ToList();
        }

        private void LoadServers()
        {
            var servers = GetPredefinedServerViewModels();
            servers.AddRange(GetServerViewModels(GetServerList()));
            Servers = servers;
        }
    }
}
