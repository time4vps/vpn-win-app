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

using System;
using System.Collections.Generic;

namespace T4VPN.Core.Network
{
    public class SafeWlanClient
    {
        private static bool _enabled;

        private readonly WlanClient _origin;

        static SafeWlanClient()
        {
            Prelink();
        }

        public SafeWlanClient(WlanClient origin)
        {
            _origin = origin;
        }

        public IReadOnlyCollection<WifiConnection> GetActiveWifiConnections()
        {
            return _enabled ? _origin.GetActiveWifiConnections() : new List<WifiConnection>(0);
        }

        private static void Prelink()
        {
            try
            {
                WlanClient.Prelink();
                _enabled = true;
            }
            catch (DllNotFoundException)
            { }
        }
    }
}
