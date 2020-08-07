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

using T4VPN.Common.Helpers;
using System.Collections.Generic;

namespace T4VPN.Core.Profiles.Comparers
{
    /// <summary>
    /// Compares only essential properties send to Profiles API. ExternalId and ColorCode is not included.
    /// </summary>
    public class ProfileByEssentialPropertiesEqualityComparer : IEqualityComparer<Profile>
    {
        public bool Equals(Profile x, Profile y)
        {
            if (ReferenceEquals(null, x)) return false;
            if (ReferenceEquals(null, y)) return false;
            if (ReferenceEquals(x, y)) return true;

            return x.ProfileType == y.ProfileType
                   && x.Name == y.Name
                   && x.Protocol == y.Protocol
                   && (x.CountryCode ?? "") == (y.CountryCode ?? "")
                   && (x.ServerId ?? "") == (y.ServerId ?? "")
                   && x.Features == y.Features;
        }

        public int GetHashCode(Profile obj)
        {
            return HashCode.Start
                .Hash(obj.ProfileType)
                .Hash(obj.Name)
                .Hash(obj.Protocol)
                .Hash(obj.CountryCode ?? "")
                .Hash(obj.ServerId ?? "")
                .Hash(obj.Features);
        }
    }
}
