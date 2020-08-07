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

using Autofac;
using T4VPN.Common.Configuration;
using T4VPN.Common.Logging;
using T4VPN.Common.OS.Net.NetworkInterface;
using T4VPN.Common.OS.Processes;
using T4VPN.Common.Threading;
using T4VPN.Vpn.Common;
using T4VPN.Vpn.Connection;
using T4VPN.Vpn.Management;
using T4VPN.Vpn.OpenVpn;
using T4VPN.Vpn.SynchronizationEvent;

namespace T4VPN.Vpn.Config
{
    public class Module
    {
        public void Load(ContainerBuilder builder)
        {
            builder.RegisterType<SystemNetworkInterfaces>().As<INetworkInterfaces>().SingleInstance();
            builder.Register(c =>
            {
                var logger = c.Resolve<ILogger>();
                var config = c.Resolve<OpenVpnConfig>();

                return new OpenVpnProcess(
                    logger,
                    c.Resolve<IOsProcesses>(),
                    new OpenVpnExitEvent(logger,
                        new SystemSynchronizationEvents(logger),
                        config.ExitEventName),
                    config);
            }
                ).SingleInstance();
        }

        public IVpnConnection VpnConnection(IComponentContext c)
        {
            var logger = c.Resolve<ILogger>();
            var config = c.Resolve<OpenVpnConfig>();
            var taskQueue = c.Resolve<ITaskQueue>();
            var networkInterfaces = c.Resolve<INetworkInterfaces>();
            var candidates = new VpnEndpointCandidates();

            var vpnConnection = new OpenVpnConnection(
                logger,
                c.Resolve<OpenVpnProcess>(),
                new ManagementClient(
                    logger,
                    new ConcurrentManagementChannel(
                        new TcpManagementChannel(
                            logger,
                            config.ManagementHost))));

            return new LoggingWrapper(
                logger,
                new DefaultGatewayWrapper(
                    logger,
                    config.TapAdapterId,
                    config.TapAdapterDescription,
                    networkInterfaces,
                    new ReconnectingWrapper(
                        logger,
                        taskQueue,
                        candidates,
                        new HandlingRequestsWrapper(
                            logger,
                            taskQueue,
                            new BestPortWrapper(
                                logger,
                                taskQueue,
                                new PingableOpenVpnPort(config.OpenVpnStaticKey),
                                new QueueingEventsWrapper(
                                    taskQueue,
                                    vpnConnection))))));
        }
    }
}