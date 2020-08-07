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

using T4VPN.Core.Servers.Models;
using System.Collections.Generic;
using System.Linq;

namespace T4VPN.Core.Servers
{
    public class RandomServer
    {
        private readonly IReadOnlyCollection<Server> _servers;

        public RandomServer(IReadOnlyCollection<Server> servers)
        {
            _servers = servers;
        }

        public Server Server()
        {
            if (!_servers.Any())
                return null;

            var random = new System.Random();
            var randomIndex = random.Next(_servers.Count);
            return _servers.ElementAt(randomIndex);
        }
    }
}
