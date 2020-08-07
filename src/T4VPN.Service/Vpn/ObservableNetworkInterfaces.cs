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

using System;
using System.Collections.Generic;
using System.Linq;
using T4VPN.Common.OS.Net.NetworkInterface;

namespace T4VPN.Service.Vpn
{
    internal class ObservableNetworkInterfaces
    {
        private readonly INetworkInterfaces _networkInterfaces;

        private List<string> _interfaces = new List<string>();

        public ObservableNetworkInterfaces(INetworkInterfaces networkInterfaces)
        {
            _networkInterfaces = networkInterfaces;

            networkInterfaces.NetworkAddressChanged += NetworkInterfaces_NetworkAddressChanged;
        }

        public event EventHandler NetworkInterfacesAdded;

        private void NetworkInterfaces_NetworkAddressChanged(object sender, EventArgs e)
        {
            var adapters = _networkInterfaces.Interfaces();
            if (!adapters.Any()) return;

            var list = adapters.Where(a => !a.IsLoopback).Select(a => a.Id).ToList();

            if (_interfaces.Any() && list.Except(_interfaces).Any())
            {
                NetworkInterfacesAdded?.Invoke(this, EventArgs.Empty);
            }

            _interfaces = list;
        }
    }
}
