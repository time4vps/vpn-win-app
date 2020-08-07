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
using Autofac;
using T4VPN.Common.Configuration;
using T4VPN.Common.Logging;
using T4VPN.Common.OS.Net.NetworkInterface;
using T4VPN.Common.OS.Processes;
using T4VPN.Common.OS.Services;
using T4VPN.Common.Service;
using T4VPN.Common.Text.Serialization;
using T4VPN.Common.Threading;
using T4VPN.Service.Firewall;
using T4VPN.Service.ServiceHosts;
using T4VPN.Service.Settings;
using T4VPN.Service.SplitTunneling;
using T4VPN.Service.Vpn;
using T4VPN.Vpn.Common;
using T4VPN.Vpn.Connection;
using Module = Autofac.Module;

namespace T4VPN.Service.Start
{
    internal class ServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.Register(c => new ConfigFactory().Config());
            builder.RegisterType<Bootstrapper>().SingleInstance();

            builder.Register(c => new NLogLoggingConfiguration(c.Resolve<Common.Configuration.Config>().ServiceLogFolder, "service"))
                .AsSelf().SingleInstance();
            builder.RegisterType<NLogLoggerFactory>().As<ILoggerFactory>().SingleInstance();
            builder.Register(c => c.Resolve<ILoggerFactory>().Logger())
                .As<ILogger>().SingleInstance();
            builder.RegisterType<LogCleaner>().SingleInstance();

            builder.RegisterType<JsonSerializerFactory>().As<ITextSerializerFactory>().SingleInstance();

            builder.RegisterType<SettingsHandler>().SingleInstance();
            builder.RegisterType<VpnConnectionHandler>().SingleInstance();

            builder.Register(c => new ServiceRetryPolicy(2, TimeSpan.FromSeconds(1))).SingleInstance();
            builder.Register(c => new CalloutDriver(
                new ReliableService(
                    c.Resolve<ServiceRetryPolicy>(),
                    new SafeService(
                        new LoggingService(
                            c.Resolve<ILogger>(),
                            new DriverService(
                                c.Resolve<Common.Configuration.Config>().SplitTunnelServiceName))))))
                .AsImplementedInterfaces().AsSelf().SingleInstance();

            builder.RegisterType<SettingsStorage>().SingleInstance();

            builder.Register(c => c.Resolve<Common.Configuration.Config>().OpenVpn).As<OpenVpnConfig>().SingleInstance();
            var vpnModule = new T4VPN.Vpn.Config.Module();
            vpnModule.Load(builder);
            builder.Register(c => 
                    new ObservableConnection(
                        new FilteringStateWrapper(
                            new QueuingRequestsWrapper(
                                c.Resolve<ITaskQueue>(),
                                new Ipv6HandlingWrapper(
                                    c.Resolve<IServiceSettings>(),
                                    c.Resolve<IFirewall>(),
                                    c.Resolve<ObservableNetworkInterfaces>(),
                                    c.Resolve<Ipv6>(),
                                    c.Resolve<ITaskQueue>(),
                                vpnModule.VpnConnection(c))))))
                .AsSelf().As<IVpnConnection>().SingleInstance();

            builder.RegisterType<ServiceSettingsHostFactory>().As<ServiceHostFactory>().SingleInstance();
            builder.RegisterType<VpnConnectionHostFactory>().As<ServiceHostFactory>().SingleInstance();
            builder.Register(c => new SerialTaskQueue()).As<ITaskQueue>().SingleInstance();
            builder.RegisterType<KillSwitch.KillSwitch>().AsImplementedInterfaces().AsSelf().SingleInstance();
            builder.RegisterType<VpnService>().SingleInstance();
            builder.RegisterType<ServiceSettings>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<Ipv6>().SingleInstance();
            builder.RegisterType<ObservableNetworkInterfaces>().SingleInstance();
            builder.RegisterType<Firewall.Firewall>().AsImplementedInterfaces().SingleInstance();
            builder.Register(c => c.Resolve<IpFilter>().Instance.CreateSublayer(
                new NetworkFilter.DisplayData{ Name = "T4VPN Firewall filters" },
                1)).SingleInstance();

            builder.RegisterType<IpFilter>().AsImplementedInterfaces().AsSelf().SingleInstance();
            builder.RegisterType<IncludeModeApps>().AsSelf().SingleInstance();
            builder.RegisterType<IpLayer>().AsSelf().SingleInstance();
            builder.Register(c => new SplitTunnel(
                c.Resolve<IServiceSettings>(),
                c.Resolve<CalloutDriver>(),
                c.Resolve<ISplitTunnelClient>(),
                c.Resolve<IncludeModeApps>(),
                c.Resolve<AppFilter>(),
                c.Resolve<PermittedRemoteAddress>()))
                .AsImplementedInterfaces()
                .AsSelf()
                .SingleInstance();
            builder.RegisterType<SystemProcesses>().As<IOsProcesses>().SingleInstance();
            builder.Register(c =>
                    new SafeSystemNetworkInterfaces(
                        c.Resolve<ILogger>(),
                        new SystemNetworkInterfaces(
                            c.Resolve<ILogger>())))
                .As<INetworkInterfaces>().SingleInstance();
            builder.RegisterType<PermittedRemoteAddress>().AsSelf().SingleInstance();
            builder.RegisterType<AppFilter>().AsSelf().SingleInstance();
            builder.RegisterType<SplitTunnelNetworkFilters>().SingleInstance();
            builder.RegisterType<BestNetworkInterface>().SingleInstance();
            builder.RegisterType<SplitTunnelClient>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<UnhandledExceptionLogging>().SingleInstance();
        }
    }
}
