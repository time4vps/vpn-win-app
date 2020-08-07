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

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace T4VPN.Core.Api.Handlers.TlsPinning
{
    public class ReportBody
    {
        private readonly List<string> _knownPins;
        private readonly X509Chain _chain;
        private readonly Uri _uri;
        private string _hash;

        public ReportBody(List<string> knownPins, Uri uri, X509Chain chain)
        {
            _knownPins = knownPins;
            _uri = uri;
            _chain = chain;
        }

        public ReportBody Value()
        {
            var certChain = GetCertificateChain();
            _hash = GetCertChainHash(certChain);

            return new ReportBody
            {
                DateTime = System.DateTime.UtcNow.ToString("s", System.Globalization.CultureInfo.InvariantCulture),
                Hostname = _uri.Host,
                Port = _uri.Port,
                ValidatedCertificateChain = certChain,
                KnownPins = _knownPins
            };
        }

        private ReportBody() { }

        public string Hash()
        {
            return _hash;
        }

        // [JsonProperty(PropertyName = "date-time")]
        public string DateTime { get; private set; }

        // [JsonProperty(PropertyName = "hostname")]
        public string Hostname { get; private set; }

        // [JsonProperty(PropertyName = "port")]
        public int Port { get; private set; }

        // [JsonProperty(PropertyName = "validated-certificate-chain")]
        public List<string> ValidatedCertificateChain { get; private set; }

        // [JsonProperty(PropertyName = "known-pins")]
        public List<string> KnownPins { get; private set; }

        private List<string> GetCertificateChain()
        {
            var list = new List<string>();

            foreach (var element in _chain.ChainElements)
            {
                list.Add(element.Certificate.ExportToPem());
            }

            return list;
        }

        private string GetCertChainHash(List<string> chain)
        {
            var stringBuilder = new StringBuilder();

            foreach (var cert in chain)
            {
                stringBuilder.Append(cert);
            }

            using (var md5 = MD5.Create())
            {
                return md5.ComputeHash(Encoding.ASCII.GetBytes(stringBuilder.ToString())).ToString();
            }
        }
    }
}
