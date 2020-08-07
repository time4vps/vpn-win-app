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

using System.Collections.Generic;
using T4VPN.Core.Servers.Models;
using T4VPN.Core.Settings;

namespace T4VPN.Core.Servers
{
    public class ServerCandidatesFactory
    {
        private readonly ServerManager _serverManager;
        private readonly IUserStorage _userStorage;

        public ServerCandidatesFactory(ServerManager serverManager, IUserStorage userStorage)
        {
            _serverManager = serverManager;
            _userStorage = userStorage;
        }

        public ServerCandidates ServerCandidates(IReadOnlyCollection<Server> servers)
        {
            return new ServerCandidates(_serverManager, _userStorage, servers);
        }
    }
}
