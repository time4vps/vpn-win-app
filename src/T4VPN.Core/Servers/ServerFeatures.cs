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

namespace T4VPN.Core.Servers
{
    public class ServerFeatures
    {
        public const int Standard = 0;
        public const int SecureCore = 1;
        public const int Tor = 2;
        public const int P2P = 4;
        public const int Xor = 8;
        public const int IpV6 = 16;

        private readonly int _value;

        public ServerFeatures(int value) => _value = value;

        public static implicit operator int(ServerFeatures item) => item._value;

        public bool IsSecureCore() => IsSecureCore(_value);
        public bool SupportsTor() => SupportsTor(_value);
        public bool SupportsP2P() => SupportsP2P(_value);

        public static bool IsSecureCore(int value) => (value & SecureCore) != 0;
        public static bool SupportsTor(int value) => (value & Tor) != 0;
        public static bool SupportsP2P(int value) => (value & P2P) != 0;
        public static bool SupportsXor(int value) => (value & Xor) != 0;
        public static bool SupportsIpV6(int value) => (value & IpV6) != 0;
    }

    [Flags]
    public enum Features
    {
        None = 0,
        SecureCore = 1,
        Tor = 2,
        P2P = 4,
    }

    public static class FeaturesExtensions
    {
        public static bool IsSecureCore(this Features value) => (value & Features.SecureCore) != 0;
        public static bool SupportsTor(this Features value) => (value & Features.Tor) != 0;
        public static bool SupportsP2P(this Features value) => (value & Features.P2P) != 0;
    }
}
