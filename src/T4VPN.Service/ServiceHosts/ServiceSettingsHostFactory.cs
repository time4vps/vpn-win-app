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

using T4VPN.Common.Logging;
using T4VPN.Common.Service;
using T4VPN.Common.ServiceModel.Server;
using T4VPN.Service.Contract.Settings;
using T4VPN.Service.Settings;
using System;

namespace T4VPN.Service.ServiceHosts
{
    public class ServiceSettingsHostFactory : ServiceHostFactory
    {
        private readonly ILogger _logger;
        private readonly SettingsHandler _proxy;

        public ServiceSettingsHostFactory(ILogger logger, SettingsHandler proxy)
        {
            _logger = logger;
            _proxy = proxy;
        }

        public override SafeServiceHost Create()
        {
            var serviceHost = new SafeServiceHost(_proxy, new Uri("net.pipe://localhost/T4VPN-service"));
            serviceHost.AddServiceEndpoint(
                typeof(ISettingsContract),
                BuildNamedPipe(),
                "settings");

            serviceHost.Description.Behaviors.Add(new ErrorLoggingBehavior(_logger));

            return serviceHost;
        }
    }
}
