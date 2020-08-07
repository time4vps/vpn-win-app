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

using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using DnsClient;
using T4VPN.Core.OS.Net.Dns;

namespace T4VPN.Core.Api.Handlers
{
    /// <summary>
    /// Suppresses expected exceptions of DnsHandler.
    /// </summary>
    public class SafeDnsHandler : DnsHandler
    {
        public SafeDnsHandler(IEventAggregator eventAggregator, IDnsClient dnsClient) : base(eventAggregator, dnsClient)
        {
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken token)
        {
            try
            {
                return await base.SendAsync(request, token);
            }
            catch (DnsResponseException)
            {
                return new FailedDnsResponse();
            }
        }
    }
}