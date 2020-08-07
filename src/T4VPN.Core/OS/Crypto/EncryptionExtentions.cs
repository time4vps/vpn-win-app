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
using System.Security.Cryptography;
using System.Text;

namespace T4VPN.Core.OS.Crypto
{
    public static class EncryptionExtentions
    {
        public static string Encrypt(this string data)
        {
            byte[] encryptedBytes = Encrypt(Encoding.UTF8.GetBytes(data));
            return Convert.ToBase64String(encryptedBytes);
        }

        public static string Decrypt(this string data)
        {
            byte[] encryptedBytes = Convert.FromBase64String(data);
            byte[] decryptedBytes = Decrypt(encryptedBytes);
            return Encoding.UTF8.GetString(decryptedBytes);
        }

        public static byte[] Encrypt(this byte[] data)
        {
            return ProtectedData.Protect(data, null, DataProtectionScope.LocalMachine);
        }

        public static byte[] Decrypt(this byte[] data)
        {
            return ProtectedData.Unprotect(data, null, DataProtectionScope.LocalMachine);
        }
    }
}
