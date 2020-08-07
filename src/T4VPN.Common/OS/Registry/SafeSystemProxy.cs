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
using T4VPN.Common.Extensions;
using T4VPN.Common.Logging;

namespace T4VPN.Common.OS.Registry
{
    public class SafeSystemProxy : ISystemProxy
    {
        private readonly ISystemProxy _proxy;
        private readonly ILogger _logger;

        public SafeSystemProxy(ILogger logger, ISystemProxy proxy)
        {
            _logger = logger;
            _proxy = proxy;
        }

        public bool Enabled()
        {
            try
            {
                return _proxy.Enabled();
            }
            catch (Exception e) when (e.IsRegistryAccessException())
            {
                _logger.Error("Can not access system proxy settings");
                _logger.Error(e);
                return false;
            }
        }
    }
}
