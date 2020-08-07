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

using T4VPN.Resources;

namespace T4VPN.Account
{
    public class PricingModel
    {
        private readonly string _name;

        public PricingModel(string name, string text, string additionalText, float monthlyPrice, float yearlyPrice, int devices, float planCompletness)
        {
            _name = name;
            MonthlyPrice = monthlyPrice;
            YearlyPrice = yearlyPrice;
            Devices = devices;
            PlanCompletness = planCompletness;
            Text = text;
            AdditionalText = additionalText;
        }

        public float MonthlyPrice { get; }
        public float YearlyPrice { get; }
        public int Devices { get; }
        public float PlanCompletness { get; }
        public bool IsUnlimited => _name == "unlimited";
        public string Text { get; }
        public string AdditionalText { get; }
        public string MonthlyPriceText => StringResources.Format("Account_lbl_MonthlyPrice", "$", MonthlyPrice);
        public string YearlyPriceText => StringResources.Format("Account_lbl_YearlyPrice", "$", YearlyPrice);
        public string Title => _name;
        public string TitleColor
        {
            get
            {
                switch (_name)
                {
                    case "trial":
                        return "#fb805f";
                    //case "vpnplus": return "#d3f421";
                    case "unlimited":
                        return "#5edefb";
                    default:
                        return "White";
                }
            }
        }
        public string DevicesText
        {
            get
            {
                var word = StringResources.GetPlural("Account_lbl_Device", Devices);
                return $"{Devices} {word}";
            }
        }
    }
}
