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

using T4VPN.Service.Settings;
using System;
using System.Collections.Generic;
using System.IO;

namespace T4VPN.Service.SplitTunneling
{
    public class IncludeModeApps
    {
        private readonly IServiceSettings _serviceSettings;

        public IncludeModeApps(IServiceSettings serviceSettings)
        {
            _serviceSettings = serviceSettings;
        }

        public string[] Value()
        {
            var list = new List<string>
            {
                Path.Combine(Environment.SystemDirectory, "svchost.exe")
            };

            if (_serviceSettings.SplitTunnelSettings.AppPaths == null)
                return list.ToArray();

            foreach (var path in _serviceSettings.SplitTunnelSettings.AppPaths)
                list.Add(path);

            return list.ToArray();
        }
    }
}
