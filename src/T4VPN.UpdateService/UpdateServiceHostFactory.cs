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
using T4VPN.Common.ServiceModel.Server;
using System;
using T4VPN.Common.Service;
using T4VPN.UpdateServiceContract;

namespace T4VPN.UpdateService
{
    internal class UpdateServiceHostFactory : ServiceHostFactory
    {
        private readonly ILogger _logger;
        private readonly UpdateHandler _proxy;

        public UpdateServiceHostFactory(ILogger logger, UpdateHandler proxy)
        {
            _logger = logger;
            _proxy = proxy;
        }

        public override SafeServiceHost Create()
        {
            var serviceHost = new SafeServiceHost(_proxy, new Uri("net.pipe://localhost/t4vpn-update-service"));

            serviceHost.AddServiceEndpoint(
                typeof(IUpdateContract),
                BuildNamedPipe(),
                "update");

            serviceHost.Description.Behaviors.Add(new ErrorLoggingBehavior(_logger));

            return serviceHost;
        }
    }
}
