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

using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using T4VPN.Common.Configuration.Api.Handlers.TlsPinning;
using T4VPN.Core.Api.Handlers.TlsPinning;

namespace T4VPN.Core.Api.Handlers
{
    /// <summary>
    /// Enforces SSL certificate validation.
    /// </summary>
    public class CertificateHandler : WebRequestHandler
    {
        private readonly TlsPinningPolicy _policy;
        private readonly IReportClient _reportClient;
        private readonly TlsPinningConfig _config;

        public CertificateHandler(TlsPinningConfig config, IReportClient reportClient)
        {
            _config = config;
            _reportClient = reportClient;
            _policy = new TlsPinningPolicy();

            ServerCertificateCustomValidationCallback = CertificateCustomValidationCallback;
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
        }

        protected bool CertificateCustomValidationCallback(HttpRequestMessage request, X509Certificate certificate,
            X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            var host = request.Headers.Host ?? request.RequestUri.Host;
            var domain = GetPinnedDomain(host) ?? GetPinnedDomain("*");
            if (domain == null)
            {
                return sslPolicyErrors == SslPolicyErrors.None && !_config.Enforce;
            }

            if (domain.Name != "*" && sslPolicyErrors != SslPolicyErrors.None)
            {
                return false;
            }

            var valid = _policy.Valid(domain, certificate);
            if (!valid && domain.SendReport)
            {
                var knownPins = domain.PublicKeyHashes.ToList();
                _reportClient.Send(new ReportBody(knownPins, request.RequestUri, chain).Value());
            }

            return sslPolicyErrors == SslPolicyErrors.None && !domain.Enforce || valid;
        }

        private TlsPinnedDomain GetPinnedDomain(string host)
        {
            return _config.PinnedDomains.FirstOrDefault(d => d.Name == host);
        }
    }
}
