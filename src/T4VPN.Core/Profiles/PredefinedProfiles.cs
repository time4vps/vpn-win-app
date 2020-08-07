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

using T4VPN.Core.Servers;
using System.Collections.Generic;

namespace T4VPN.Core.Profiles
{
    public class PredefinedProfiles : IProfileSource
    {
        public IReadOnlyList<Profile> GetAll()
        {
            var profiles = new List<Profile>
            {
                new Profile("Fastest") { IsPredefined = true, Name = "Fastest", ProfileType = ProfileType.Fastest, Features = Features.None },
                new Profile("Random") { IsPredefined = true, Name = "Random", ProfileType = ProfileType.Random, Features = Features.None }
            };
            return profiles;
        }
    }
}
