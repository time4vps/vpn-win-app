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

using DnsClient;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using T4VPN.Common.Helpers;

namespace T4VPN.Core.OS.Net.Dns
{
    /// <summary>
    /// Wrapper around third party DNS client.
    /// Gets DNS servers on construction and uses them throughout lifetime.
    /// </summary>
    public class FixedDnsClient : IDnsClient
    {
        private readonly ILookupClient _lookupClient;

        internal FixedDnsClient(ILookupClient lookupClient)
        {
            Ensure.NotNull(lookupClient, nameof(lookupClient));

            _lookupClient = lookupClient;
        }

        public async Task<string> Resolve(string host, CancellationToken token)
        {
            if (IPAddress.TryParse(host, out _))
                return host;

            var result = await _lookupClient.QueryAsync(host, QueryType.A, cancellationToken: token);
            if (result.HasError)
                return null;

            var record = result.Answers.ARecords().FirstOrDefault();

            return record?.Address.MapToIPv4().ToString();
        }

        public IReadOnlyCollection<IPEndPoint> NameServers =>
            _lookupClient.NameServers.Select(s => new IPEndPoint(IPAddress.Parse(s.Address), s.Port)).ToArray();
    }
}
