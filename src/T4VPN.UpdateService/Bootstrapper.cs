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
using System.IO;
using System.ServiceModel;
using System.ServiceProcess;
using Autofac;
using T4VPN.Common.Configuration;
using T4VPN.Common.CrashReporting;
using T4VPN.Common.Logging;
using T4VPN.Common.OS.Event;
using T4VPN.Common.OS.Net.Http;
using T4VPN.Common.OS.Processes;
using T4VPN.Core.Api;
using T4VPN.Core.Api.Handlers;
using T4VPN.Core.Api.Handlers.TlsPinning;
using T4VPN.Core.OS.Net;
using T4VPN.Update;
using T4VPN.Update.Config;
using Sentry;
using Sentry.Protocol;

namespace T4VPN.UpdateService
{
    internal class Bootstrapper
    {
        private IContainer _container;

        private T Resolve<T>() => _container.Resolve<T>();

        public void Initialize()
        {
            Configure();
            InitCrashLogging();
            InitCrashReporting();
            Start();
        }

        public void StartUpdate()
        {
            Configure();
            InitCrashLogging();
            InitCrashReporting();

            try
            {
                var content = System.IO.File.ReadAllText(Resolve<Config>().UpdateFilePath).Split('\n');
                if (content.Length != 2)
                {
                    return;
                }

                StartElevatedProcess(content[0], content[1]);
            }
            catch (Exception e)
            {
                SentrySdk.WithScope(scope =>
                {
                     scope.Level = SentryLevel.Error;
                     scope.SetTag("captured_in", "UpdateService_Bootstrapper_StartUpdate");
                     SentrySdk.CaptureException(e);
                });
            }
        }

        private void StartElevatedProcess(string path, string arguments)
        {
            Resolve<IOsProcesses>().ElevatedProcess(path, arguments).Start();
        }

        private void InitCrashLogging()
        {
            var logging = Resolve<UnhandledExceptionLogging>();
            logging.CaptureUnhandledExceptions();
            logging.CaptureTaskExceptions();
        }

        private void Start()
        {
            var config = Resolve<Config>();
            var logger = Resolve<ILogger>();

            logger.Info($"= Booting T4VPN Update Service version: {config.AppVersion} os: {Environment.OSVersion.VersionString} {config.OsBits} bit =");

            Resolve<ServicePointConfiguration>().Apply();
            CreateLogFolder();
            Resolve<IAppUpdates>().Cleanup();

            ServiceBase.Run(Resolve<UpdateService>());

            logger.Info("= T4VPN Update Service has exited =");
        }

        private void CreateLogFolder()
        {
            Directory.CreateDirectory(Resolve<Config>().UpdateServiceLogFolder);
        }

        private void InitCrashReporting()
        {
            CrashReports.Init(Resolve<Config>(), Resolve<ILogger>());
        }

        private void Configure()
        {
            var builder = new ContainerBuilder();
            builder.Register(c => new ConfigFactory().Config());
            builder.RegisterType<SystemProcesses>().As<IOsProcesses>().SingleInstance();
            builder.RegisterType<HttpClients>().As<IHttpClients>().SingleInstance();

            builder.Register(c => new NLogLoggingConfiguration(c.Resolve<Config>().UpdateServiceLogFolder, "updater"))
                .AsSelf().SingleInstance();
            builder.RegisterType<NLogLoggerFactory>().As<ILoggerFactory>().SingleInstance();
            builder.Register(c => c.Resolve<ILoggerFactory>().Logger())
                .As<ILogger>().SingleInstance();

            builder.Register(c => c.Resolve<UpdateServiceHostFactory>().Create()).As<ServiceHost>().SingleInstance();
            builder.RegisterType<EventBasedLaunchableFile>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<UpdateHandler>().SingleInstance();
            builder.RegisterType<UpdateServiceHostFactory>().SingleInstance();
            builder.RegisterType<UpdateService>().SingleInstance();
            builder.RegisterType<ServicePointConfiguration>().SingleInstance();
            builder.RegisterType<SystemEventLog>().SingleInstance();
            builder.RegisterType<UnhandledExceptionLogging>().SingleInstance();

            builder.Register(c =>
                    new CachingReportClient(
                        new ReportClient(new Uri(c.Resolve<Config>().Urls.TlsReportUrl))))
                .As<IReportClient>()
                .SingleInstance();

            builder.Register(c =>
                new CertificateHandler(
                    c.Resolve<Config>().TlsPinningConfig,
                    c.Resolve<IReportClient>())).SingleInstance();

            builder.Register(c =>
                new LoggingHandler(
                        c.Resolve<ILogger>())
                    { InnerHandler = c.Resolve<CertificateHandler>() });

            builder.Register(c =>
                new RetryingHandler(
                        c.Resolve<Config>().ApiTimeout,
                        c.Resolve<Config>().ApiRetries,
                        (retryCount, response, context) => new SleepDurationProvider(response).Value())
                    { InnerHandler = c.Resolve<LoggingHandler>() }).SingleInstance();

            builder.Register(c => new DefaultAppUpdateConfig
                {
                    HttpClient = c.Resolve<IHttpClients>().Client(c.Resolve<RetryingHandler>()),
                    FeedUri = new Uri(c.Resolve<Config>().Urls.UpdateUrl),
                    UpdatesPath = c.Resolve<Config>().UpdatesPath,
                    CurrentVersion = Version.Parse(c.Resolve<Config>().AppVersion),
                    EarlyAccessCategoryName = "EarlyAccess",
                    MinProgressDuration = TimeSpan.FromSeconds(1.5)
                })
                .As<IAppUpdateConfig>().SingleInstance();

            new Update.Config.Module().Load(builder);

            _container = builder.Build();
        }
    }
}
