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
using System.ServiceProcess;
using System.Threading.Tasks;
using Autofac;
using T4VPN.Common.Configuration;
using T4VPN.Common.CrashReporting;
using T4VPN.Common.Logging;
using T4VPN.Common.OS.Processes;
using T4VPN.Common.Vpn;
using T4VPN.Native.PInvoke;
using T4VPN.Service.Config;
using T4VPN.Service.Settings;
using T4VPN.Service.Vpn;
using T4VPN.Vpn.Common;
using T4VPN.Vpn.OpenVpn;

namespace T4VPN.Service.Start
{
    internal class Bootstrapper
    {
        private IContainer _container;

        private T Resolve<T>() => _container.Resolve<T>();

        public void Initialize()
        {
            SetDllDirectories();
            Configure();
            Start();
        }

        private void Configure()
        {
            var config = new ConfigFactory().Config();
            new ConfigDirectories(config).Prepare();

            var builder = new ContainerBuilder();
            builder.RegisterModule<ServiceModule>();
            _container = builder.Build();
        }

        private void Start()
        {
            var config = Resolve<Common.Configuration.Config>();
            var logger = Resolve<ILogger>();

            logger.Info($"= Booting T4VPN Service version: {config.AppVersion} os: {Environment.OSVersion.VersionString} {config.OsBits} bit =");

            Resolve<UnhandledExceptionLogging>().CaptureTaskExceptions();
            Resolve<UnhandledExceptionLogging>().CaptureUnhandledExceptions();

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            InitCrashReporting();
            RegisterEvents();

            Resolve<LogCleaner>().Clean(config.ServiceLogFolder, 30);
            ServiceBase.Run(Resolve<VpnService>());

            logger.Info("= T4VPN Service has exited =");
        }

        private void InitCrashReporting()
        {
            CrashReports.Init(Resolve<Common.Configuration.Config>(), Resolve<ILogger>());
        }

        private void RegisterEvents()
        {
            Resolve<ObservableConnection>().AfterStateChanged += (s, e) =>
            {
                var state = e.Data;
                var instances = Resolve<IEnumerable<IVpnStateAware>>();
                foreach (var instance in instances)
                {
                    switch (state.Status)
                    {
                        case VpnStatus.Connecting:
                        case VpnStatus.Reconnecting:
                            instance.OnVpnConnecting(state);
                            break;
                        case VpnStatus.Connected:
                            instance.OnVpnConnected(state);
                            break;
                        case VpnStatus.Disconnecting:
                        case VpnStatus.Disconnected:
                            instance.OnVpnDisconnected(state);
                            break;
                    }
                }
            };

            Resolve<IServiceSettings>().SettingsChanged += (s, e) =>
            {
                var instances = Resolve<IEnumerable<IServiceSettingsAware>>();
                foreach (var instance in instances)
                {
                    instance.OnServiceSettingsChanged(e);
                }
            };
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var config = Resolve<Common.Configuration.Config>();
            var processes = Resolve<IOsProcesses>();
            Resolve<IVpnConnection>().Disconnect();
            Resolve<OpenVpnProcess>().Stop();
            processes.KillProcesses(config.AppName);
        }

        private static void SetDllDirectories()
        {
            Kernel32.SetDefaultDllDirectories(Kernel32.SetDefaultDllDirectoriesFlags.LOAD_LIBRARY_SEARCH_DEFAULT_DIRS);
        }
    }
}
