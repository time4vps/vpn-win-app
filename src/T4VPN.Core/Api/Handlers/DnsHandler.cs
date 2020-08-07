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

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using T4VPN.Common.Vpn;
using T4VPN.Core.OS.Net.Dns;
using T4VPN.Core.Vpn;

namespace T4VPN.Core.Api.Handlers
{
    /// <summary>
    /// Replaces host with IP in Http requests if network is blocked and VPN is not connected.
    /// </summary>
    /// <remarks>
    /// If an app asks Windows to resolve IP, the DNS requests are send by different process than this app.
    /// If VPN is not connected and firewall is blocking system DNS requests, DNS should be resolved by
    /// by the app itself to pass through firewall.
    /// </remarks>
    public class DnsHandler : DelegatingHandler, IHandle<VpnStateChangedEventArgs>
    {
        private readonly IDnsClient _dnsClient;

        private bool _systemDnsBlocked;

        public DnsHandler(IEventAggregator eventAggregator, IDnsClient dnsClient)
        {
            _dnsClient = dnsClient;

            eventAggregator.Subscribe(this);
        }

        public void Handle(VpnStateChangedEventArgs e)
        {
            _systemDnsBlocked = e.NetworkBlocked && e.State.Status != VpnStatus.Connected;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken token)
        {
            if (_systemDnsBlocked)
            {
                request = await ModifyRequest(request, token);
            }

            return await base.SendAsync(request, token);
        }

        private async Task<HttpRequestMessage> ModifyRequest(HttpRequestMessage message, CancellationToken token)
        {
            var ip = await _dnsClient.Resolve(message.RequestUri.Host, token);
            if (!string.IsNullOrEmpty(ip))
            {
                var uriBuilder = new UriBuilder(message.RequestUri) { Host = ip };
                message.Headers.Host = message.RequestUri.Host;
                message.RequestUri = uriBuilder.Uri;
            }

            return message;
        }
    }
}