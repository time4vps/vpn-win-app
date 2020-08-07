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
using System.Threading.Tasks;
using T4VPN.Common.OS.Services;
using T4VPN.Core.Service;

namespace T4VPN.Modals
{
    public class ServiceStartModalViewModel : BaseModalViewModel
    {
        private static readonly TimeSpan DelayBetweenServiceStartAttempts = TimeSpan.FromSeconds(3);

        private readonly VpnSystemService _service;

        private bool _starting;

        public ServiceStartModalViewModel(VpnSystemService service)
        {
            _service = service;
        }

        public override async void BeforeOpenModal(dynamic options)
        {
            base.OnInitialize();

            _starting = true;

            while (_starting)
            {
                var result = await _service.StartAsync();
                if (result.Success)
                {
                    TryClose(true);
                    break;
                }

                await Task.Delay(DelayBetweenServiceStartAttempts);
            }
        }

        public override void CloseAction()
        {
            _starting = false;

            TryClose(false);
        }
    }
}
