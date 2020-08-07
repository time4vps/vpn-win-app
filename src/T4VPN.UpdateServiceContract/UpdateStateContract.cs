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

namespace T4VPN.UpdateServiceContract
{
    [DataContract]
    public class UpdateStateContract
    {
        public UpdateStateContract(
            ICollection<ReleaseContract> releaseHistory,
            bool available,
            bool ready,
            AppUpdateStatusContract status,
            string filePath,
            string fileArguments)
        {
            ReleaseHistory = releaseHistory;
            Available = available;
            Ready = ready;
            Status = status;
            FilePath = filePath;
            FileArguments = fileArguments;
        }

        [DataMember]
        public ICollection<ReleaseContract> ReleaseHistory { get; private set; }

        [DataMember]
        public bool Available { get; private set; }

        [DataMember]
        public bool Ready { get; private set; }

        [DataMember]
        public AppUpdateStatusContract Status { get; private set; }

        [DataMember]
        public string FilePath { get; private set; }

        [DataMember]
        public string FileArguments { get; private set; }
    }
}
