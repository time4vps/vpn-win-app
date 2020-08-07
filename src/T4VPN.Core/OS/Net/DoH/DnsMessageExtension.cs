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

using System.Reflection;
using ARSoft.Tools.Net.Dns;

namespace T4VPN.Core.OS.Net.DoH
{
    public static class DnsMessageExtension
    {
        public static byte[] Encode(this DnsMessage message)
        {
            var m = message.GetType().GetMethod(
                "Encode",
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new[] { typeof(bool), typeof(byte[]).MakeByRefType() },
                null);

            var args = new object[] { false, null };
            m.Invoke(message, args);

            return args[1] as byte[];
        }
    }
}
