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
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace T4VPN.Core.OS.Net.Dns
{
    public class NullDnsClient : IDnsClient
    {
        internal NullDnsClient()
        { }

        public Task<string> Resolve(string host, CancellationToken token) => 
            Task.FromResult<string>(null);

        public IReadOnlyCollection<IPEndPoint> NameServers => new IPEndPoint[0];
    }
}
