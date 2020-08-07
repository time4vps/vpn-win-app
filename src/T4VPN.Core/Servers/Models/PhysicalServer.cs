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

namespace T4VPN.Core.Servers.Models
{
    public class PhysicalServer
    {
        public string EntryIp { get; }
        public string ExitIp { get; }
        public string Domain { get; }
        public sbyte Status { get; }

        public PhysicalServer(string entryIp, string exitIp, string domain, sbyte status)
        {
            EntryIp = entryIp;
            ExitIp = exitIp;
            Domain = domain;
            Status = status;
        }
    }
}
