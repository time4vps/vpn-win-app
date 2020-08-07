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

using T4VPN.Core.Servers;
using T4VPN.Core.Settings;
using System.Collections.Generic;
using System.Linq;

namespace T4VPN.Servers
{
    public class SortedCountries
    {
        private readonly IUserStorage _userStorage;
        private readonly ServerManager _serverManager;

        public SortedCountries(IUserStorage userStorage, ServerManager serverManager)
        {
            _serverManager = serverManager;
            _userStorage = userStorage;
        }

        public List<string> List()
        {
            var user = _userStorage.User();
            var allCountries = _serverManager.GetCountries().OrderBy(Countries.GetName);

            if (!user.MaxTier.Equals(ServerTiers.Free))
                return allCountries.ToList();

            var freeCountries = _serverManager.GetCountriesWithFreeServers()
                .OrderByDescending(Countries.GetName)
                .ToList();

            return freeCountries.Concat(allCountries.Except(freeCountries)).ToList();
        }
    }
}
