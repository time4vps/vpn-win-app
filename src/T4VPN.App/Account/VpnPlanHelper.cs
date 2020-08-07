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

using T4VPN.Common.Extensions;
using T4VPN.Resources;

namespace T4VPN.Account
{
    public class VpnPlanHelper
    {
        public static string GetPlanName(string vpnPlan)
        {
            switch (vpnPlan)
            {
                case "free":
                case "unlimited":
                case "trial":
                    return StringResources.Get($"VpnPlan_val_{vpnPlan.FirstCharToUpper()}");
                default:
                    return vpnPlan;
            }
        }

        public static string GetPlanColor(string vpnPlan)
        {
            switch (vpnPlan)
            {
                case "free":
                    return "White";
                case "trial":
                    return "#8ec122";
                case "unlimited":
                    return "#54d8fd";
                default:
                    return "White";
            }
        }
    }
}
