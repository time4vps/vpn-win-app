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

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.X509;

namespace T4VPN.Core.Api.Extensions
{
    public static class X509CertificateExtensions
    {
        public static byte[] GetSubjectPublicKeyInfo(this System.Security.Cryptography.X509Certificates.X509Certificate certificate)
        {
            var cert = new X509CertificateParser().ReadCertificate(certificate.GetRawCertData());
            var tbsCert = TbsCertificateStructure.GetInstance(Asn1Object.FromByteArray(cert.GetTbsCertificate()));
            var info = tbsCert.SubjectPublicKeyInfo.GetDerEncoded();
            return info;
        }
    }
}
