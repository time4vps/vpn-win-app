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
using T4VPN.Core.User;

namespace T4VPN.Core.Models
{
    public class User
    {
        public string VpnUsername { get; set; }
        public string Username { get; set; }
        public string VpnPassword { get; set; }
        public string VpnPlan { get; set; }
        public sbyte MaxTier { get; set; }
        public int Services { get; set; }
        public int ExpirationTime { get; set; }
        public int Delinquent { get; set; }
        public int MaxConnect { get; set; }

        public string GetAccountPlan()
        {
            switch (Services)
            {
                case 1:
                    return "Free Account";
                case 2:
                    return "Trial Account";
                case 3:
                    return "Paid Account";
                default:
                    return "?";
            }
        }

        public PlanStatus TrialStatus()
        {
            switch (VpnPlan)
            {
                case null:
                    return PlanStatus.Free;
                case "trial" when ExpirationTime == 0:
                    return PlanStatus.TrialNotStarted;
                case "trial" when ExpirationTime > 0:
                    return PlanStatus.TrialStarted;
                default:
                    return Paid() ? PlanStatus.Paid : PlanStatus.Free;
            }
        }

        public bool Paid()
        {
            return VpnPlan != null && !VpnPlan.Equals("trial") && !VpnPlan.Equals("free");
        }

        public long TrialExpirationTimeInSeconds()
        {
            var now = ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds();
            var secondsLeft = ExpirationTime - now;
            return secondsLeft < 0 ? 0 : secondsLeft;
        }

        public bool Empty()
        {
            return string.IsNullOrEmpty(Username);
        }

        public static User EmptyUser()
        {
            return new User();
        }
    }
}
