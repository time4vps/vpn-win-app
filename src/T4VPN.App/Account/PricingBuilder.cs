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

using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using T4VPN.Core.Api;
using T4VPN.Core.Api.Contracts;
using T4VPN.Core.Servers;

namespace T4VPN.Account
{
    public class PricingBuilder
    {
        private readonly Common.Configuration.Config _appConfig;
        private readonly IApiClient _api;
        private readonly ServerManager _serverManager;
        private Dictionary<string, Pricing> _monthlyPlans = new Dictionary<string, Pricing>();
        private Dictionary<string, Pricing> _yearlyPlans = new Dictionary<string, Pricing>();
        private float _mostExpensivePlanPrice;


        public PricingBuilder(Common.Configuration.Config appConfig, IApiClient api, ServerManager serverManager)
        {
            _appConfig = appConfig;
            _api = api;
            _serverManager = serverManager;
        }

        public async Task Load()
        {
            await LoadMonthlyPriceInfo();
            await LoadYearlyPriceInfo();
            SetMostExpensivePlanPrice();
        }

        public List<PricingModel> BuildPricing()
        {
            var pricing = new List<PricingModel> { };

            foreach (var monthlyPlan in _monthlyPlans)
            {
                if (_yearlyPlans.ContainsKey(monthlyPlan.Key))
                {
                    var plan = monthlyPlan.Value;
                    var yearlyPrice = _yearlyPlans[monthlyPlan.Key].Price;

                    pricing.Add(new PricingModel(
                        plan.Name,
                        plan.Text,
                        plan.AdditionalText,
                        plan.Price,
                        yearlyPrice,
                        plan.MaxVpn,
                        GetPlanCompletness(plan)));
                }
            }

            return pricing;
        }

        private async Task LoadMonthlyPriceInfo()
        {
            try
            {
                var response = await _api.GetPricing(_appConfig.DefaultCurrency, 1);
                if (response.Success)
                    _monthlyPlans = GetPricingDictionary(response.Value);
            }
            catch (HttpRequestException)
            {
                throw new PricingBuilderException();
            }
        }

        private async Task LoadYearlyPriceInfo()
        {
            try
            {
                var response = await _api.GetPricing(_appConfig.DefaultCurrency, 12);
                if (response.Success)
                    _yearlyPlans = GetPricingDictionary(response.Value);
            }
            catch (HttpRequestException)
            {
                throw new PricingBuilderException();
            }
        }

        private static Dictionary<string, Pricing> GetPricingDictionary(PricingPlans pricing)
        {
            return pricing.Plans.ToDictionary(plan => plan.Id);
        }

        private float GetPlanCompletness(Pricing pricing)
        {
            return pricing.Price / _mostExpensivePlanPrice;
        }

        private void SetMostExpensivePlanPrice()
        {
            foreach (var plan in _monthlyPlans)
            {
                if (plan.Value.Price > _mostExpensivePlanPrice)
                    _mostExpensivePlanPrice = plan.Value.Price;
            }
        }
    }
}
