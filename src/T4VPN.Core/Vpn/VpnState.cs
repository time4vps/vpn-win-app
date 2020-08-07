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

using T4VPN.Common.Vpn;
using T4VPN.Core.Servers.Models;

namespace T4VPN.Core.Vpn
{
    public class VpnState
    {
        public VpnStatus Status { get; }
        public string EntryIp { get; }
        public Server Server { get; }
        public VpnProtocol Protocol { get; }

        public VpnState(VpnStatus status, string entryIp, VpnProtocol protocol)
        {
            Status = status;
            EntryIp = entryIp;
            Protocol = protocol;
        }

        public VpnState(VpnStatus status, Server server = null)
        {
            Status = status;
            Server = server ?? Server.Empty();
        }

        public override string ToString()
        {
            return $"Status: {Status}. Server: {Server?.ToString() ?? "None"}";
        }
    }
}
