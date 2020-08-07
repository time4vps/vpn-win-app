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

using System.Collections.Generic;
using Newtonsoft.Json;

namespace T4VPN.Core.Api.Contracts
{
    public class VpnConfig : BaseResponse
    {
        public OpenVpnConfig OpenVpnConfig { get; set; }

        [JsonProperty(PropertyName = "HolesIPs")]
        public IReadOnlyList<string> HolesIps { get; set; }

        public FeatureFlags FeatureFlags { get; set; }
    }

    public class OpenVpnConfig
    {
        public Ports DefaultPorts { get; set; }

        public Ports XorPorts { get; set; }
    }

    public class Ports
    {
        public int[] Udp { get; set; }
        public int[] Tcp { get; set; }
    }

    public class FeatureFlags
    {
        public bool NetShield { get; set; }
        public bool GuestHoles { get; set; }
    }
}
