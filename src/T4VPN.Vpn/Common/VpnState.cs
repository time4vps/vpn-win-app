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

using T4VPN.Common.Vpn;

namespace T4VPN.Vpn.Common
{
    public class VpnState
    {
        public VpnState(VpnStatus status) : this(status, VpnError.None, string.Empty, string.Empty)
        { }

        public VpnState(VpnStatus status, VpnError error) : this(status, error, string.Empty, string.Empty)
        { }

        public VpnState(VpnStatus status, VpnError error, string localIp, string remoteIp, VpnProtocol protocol = VpnProtocol.Auto)
        {
            Status = status;
            Error = error;
            LocalIp = localIp;
            RemoteIp = remoteIp;
            Protocol = protocol;
        }

        public VpnStatus Status { get; }

        public VpnError Error { get; }

        public string LocalIp { get; }

        public string RemoteIp { get; }

        public VpnProtocol Protocol { get; }
    }
}
