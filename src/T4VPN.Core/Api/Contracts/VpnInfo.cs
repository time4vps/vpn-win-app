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

namespace T4VPN.Core.Api.Contracts
{
    public class VpnInfoResponse : BaseResponse
    {
        public int Services { get; set; }

        public int Delinquent { get; set; }

        public VpnInfo Vpn { get; set; }
    }

    public class VpnInfo
    {
        public string PlanName { get; set; }

        public string Name { get; set; }

        // 0 = no vpn access, 1 = vpn access, 2 = vpn access requested (waitlist)
        public int Status { get; set; }

        public int ExpirationTime { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public sbyte MaxTier { get; set; }

        public int MaxConnect { get; set; }

        [JsonProperty(PropertyName = "GroupID")]
        public string GroupId { get; set; }

        public string Password { get; set; }
    }
}
