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
using T4VPN.NetworkFilter;
using Action = T4VPN.NetworkFilter.Action;

namespace T4VPN.Service.Firewall
{
    public class PermittedRemoteAddress : IFilterCollection
    {
        private readonly IpLayer _ipLayer;
        private readonly Sublayer _sublayer;

        private readonly Dictionary<string,List<Guid>> _list = new Dictionary<string, List<Guid>>();

        public PermittedRemoteAddress(Sublayer sublayer, IpLayer ipLayer)
        {
            _ipLayer = ipLayer;
            _sublayer = sublayer;
        }

        public void Add(string[] addresses, Action action)
        {
            foreach (var address in addresses)
                Add(address, action);
        }

        public void Add(string address, Action action)
        {
            if (_list.ContainsKey(address))
                return;

            var networkAddress = new Common.OS.Net.NetworkAddress(address);
            if (!networkAddress.Valid())
            {
                return;
            }

            _list[address] = new List<Guid>();

            _ipLayer.ApplyToIpv4(layer =>
            {
                var guid = _sublayer.CreateRemoteNetworkIPv4Filter(
                    new DisplayData("T4VPN permit remote address", ""),
                    action,
                    layer,
                    14,
                    new NetworkAddress(networkAddress.Ip, networkAddress.Mask));

                _list[address].Add(guid);
            });
        }

        public void Remove(string address)
        {
            if (!_list.ContainsKey(address))
                return;

            foreach (var guid in _list[address])
                _sublayer.DestroyFilter(guid);

            _list.Remove(address);
        }

        public void RemoveAll()
        {
            if (_list.Count == 0)
                return;

            foreach (var element in _list.ToList())
                Remove(element.Key);
        }
    }
}
