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

namespace T4VPN.Core.OS.Net.DoH
{
    public class DohClients
    {
        private readonly List<Client> _list = new List<Client>();

        private readonly List<string> _providers;
        private readonly TimeSpan _timeout;

        public DohClients(List<string> providers, TimeSpan timeout)
        {
            _providers = providers;
            _timeout = timeout;
        }

        public List<Client> Get()
        {
            if (_list.Count > 0)
            {
                return _list;
            }

            foreach (var provider in _providers)
            {
                _list.Add(new Client(provider, _timeout));
            }

            return _list;
        }
    }
}
