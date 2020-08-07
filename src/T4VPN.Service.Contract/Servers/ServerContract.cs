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

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace T4VPN.Service.Contract.Servers
{
    [DataContract]
    public class ServerContract
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string EntryCountry { get; set; }

        [DataMember]
        public string ExitCountry { get; set; }

        [DataMember]
        public sbyte Features { get; set; }

        [DataMember]
        public int Tier { get; set; }

        [DataMember]
        public ICollection<PhysicalServerContract> Servers { get; set; }
    }
}
