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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using T4VPN.NetworkFilter;
using Action = T4VPN.NetworkFilter.Action;

namespace T4VPN.Service.Firewall
{
    public class AppFilter : IFilterCollection
    {
        private readonly Sublayer _sublayer;
        private readonly IpLayer _ipLayer;
        private readonly Dictionary<string, List<Guid>> _list = new Dictionary<string, List<Guid>>();

        public AppFilter(Sublayer sublayer, IpLayer ipLayer)
        {
            _ipLayer = ipLayer;
            _sublayer = sublayer;
        }

        public void Add(string[] paths, Action action)
        {
            foreach (var path in paths)
                Add(path, action);
        }

        public void Add(string path, Action action)
        {
            if (_list.ContainsKey(path))
                return;

            if (!File.Exists(path))
                return;

            _list[path] = new List<Guid>();

            _ipLayer.ApplyToIpv4(layer =>
            {
                var guid = _sublayer.CreateAppFilter(
                    new DisplayData("Time4VPS VPN permit app", "Allow app to bypass VPN tunnel"),
                    action,
                    layer,
                    14,
                    path);

                _list[path].Add(guid);
            });
        }

        public void Remove(string path)
        {
            if (!_list.ContainsKey(path))
                return;

            foreach (var guid in _list[path])
                _sublayer.DestroyFilter(guid);

            _list.Remove(path);
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
