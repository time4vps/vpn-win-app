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
using System.Net.Http;
using Autofac;
using Caliburn.Micro;
using T4VPN.Common.Extensions;
using T4VPN.Common.Logging;
using T4VPN.Common.OS;
using T4VPN.Common.OS.Net.Http;
using T4VPN.Common.OS.Net.NetworkInterface;
using T4VPN.Common.OS.Processes;
using T4VPN.Common.OS.Registry;
using T4VPN.Common.Threading;
using T4VPN.Config.Url;
using T4VPN.Core.Abstract;
using T4VPN.Core.Api;
using T4VPN.Core.Api.Handlers;
using T4VPN.Core.Api.Handlers.TlsPinning;
using T4VPN.Core.Auth;
using T4VPN.Core.Events;
using T4VPN.Core.Network;
using T4VPN.Core.OS.Net;
using T4VPN.Core.OS.Net.Dns;
using T4VPN.Core.OS.Net.DoH;
using T4VPN.Core.Servers;
using T4VPN.Core.Settings;
using T4VPN.Core.Storage;
using T4VPN.Core.Threading;
using T4VPN.Core.Update;
using T4VPN.Resources;
using T4VPN.Vpn;
using Module = Autofac.Module;

namespace T4VPN.Core.Ioc
{
    public class CoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<ServicePointConfiguration>().SingleInstance();
            builder.RegisterType<ServerManager>().SingleInstance();
            builder.RegisterType<ActiveUrls>().As<IActiveUrls>().SingleInstance();
            builder.RegisterType<ApiAppVersion>().As<IApiAppVersion>().SingleInstance();
            builder.RegisterType<OsEnvironment>().As<IEnvironment>().SingleInstance();
            builder.RegisterType<SystemProcesses>().As<IOsProcesses>().SingleInstance();
            builder.Register(c => 
                    new SafeSystemNetworkInterfaces(
                        c.Resolve<ILogger>(),
                        new SystemNetworkInterfaces(
                            c.Resolve<ILogger>())))
                .As<INetworkInterfaces>().SingleInstance();
            builder.RegisterType<HttpClients>().As<IHttpClients>().SingleInstance();
            builder.Register(c => Schedulers.FromApplicationDispatcher()).As<IScheduler>().SingleInstance();
            builder.Register(c => new TokenStorage(c.Resolve<UserSettings>())).As<ITokenStorage>().SingleInstance();

            builder.Register(c =>
                new CertificateHandler(
                    c.Resolve<Common.Configuration.Config>().TlsPinningConfig,
                    c.Resolve<IReportClient>())).SingleInstance();

            builder.Register(c => 
                new SafeDnsHandler(
                        c.Resolve<IEventAggregator>(),
                        c.Resolve<IDnsClient>())
                    { InnerHandler = c.Resolve<CertificateHandler>() }).SingleInstance();

            builder.Register(c =>
                new LoggingHandler(
                        c.Resolve<ILogger>())
                    {InnerHandler = c.Resolve<SafeDnsHandler>()});

            builder.Register(c =>
                new RetryingHandler(
                        c.Resolve<Common.Configuration.Config>().ApiTimeout,
                        c.Resolve<Common.Configuration.Config>().ApiRetries,
                        (retryCount, response, context) => new SleepDurationProvider(response).Value())
                    {InnerHandler = c.Resolve<LoggingHandler>()}).SingleInstance();

            builder.Register(c =>
                new OutdatedAppHandler
                    {InnerHandler = c.Resolve<RetryingHandler>()}).SingleInstance();

            builder.Register(c =>
                new UnauthorizedResponseHandler(
                        c.Resolve<ITokenClient>(),
                        c.Resolve<ITokenStorage>(),
                        c.Resolve<IUserStorage>())
                    {InnerHandler = c.Resolve<OutdatedAppHandler>()}).SingleInstance();

            builder.Register(c =>
                new CancellingHandler
                    {InnerHandler = c.Resolve<UnauthorizedResponseHandler>()}).SingleInstance();

            builder.Register(c =>
                new AlternativeHostHandler(c.Resolve<DohClients>(),
                        c.Resolve<MainHostname>(),
                        c.Resolve<IAppSettings>(),
                        c.Resolve<GuestHoleState>(),
                        new Uri(c.Resolve<Common.Configuration.Config>().Urls.ApiUrl).Host)
                    { InnerHandler = c.Resolve<CancellingHandler>() }).SingleInstance();

            builder.Register(c =>
                    new TokenClient(
                        new HttpClient(c.Resolve<RetryingHandler>())
                            {BaseAddress = c.Resolve<IActiveUrls>().ApiUrl.Uri},
                        c.Resolve<IApiAppVersion>(),
                        c.Resolve<ITokenStorage>(),
                        c.Resolve<Common.Configuration.Config>().ApiVersion,
                        TranslationSource.Instance.CurrentCulture.Name))
                .As<ITokenClient>()
                .SingleInstance();

            builder.Register(c =>
                {
                    var certificateHandler = new CertificateHandler(
                        c.Resolve<Common.Configuration.Config>().TlsPinningConfig,
                        c.Resolve<IReportClient>());

                    var safeDnsHandler = new SafeDnsHandler(
                            c.Resolve<IEventAggregator>(),
                            c.Resolve<IDnsClient>())
                        {InnerHandler = certificateHandler};

                    var loggingHandler = new LoggingHandler(
                            c.Resolve<ILogger>())
                        {InnerHandler = safeDnsHandler};

                    var retryingHandler = new RetryingHandler(
                            c.Resolve<Common.Configuration.Config>().ApiTimeout,
                            c.Resolve<Common.Configuration.Config>().ApiRetries,
                            (retryCount, response, context) => new SleepDurationProvider(response).Value())
                        {InnerHandler = loggingHandler};

                    var alternativeHostHandler = new AlternativeHostHandler(
                        c.Resolve<DohClients>(),
                        c.Resolve<MainHostname>(),
                        c.Resolve<IAppSettings>(),
                        c.Resolve<GuestHoleState>(),
                        new Uri(c.Resolve<Common.Configuration.Config>().Urls.ApiUrl).Host)
                    { InnerHandler = retryingHandler};

                    return new ApiClient(
                        new HttpClient(c.Resolve<AlternativeHostHandler>())
                        {
                            BaseAddress = c.Resolve<IActiveUrls>().ApiUrl.Uri
                        },
                        new HttpClient(alternativeHostHandler)
                        {
                            BaseAddress = c.Resolve<IActiveUrls>().ApiUrl.Uri,
                            DefaultRequestHeaders = { ConnectionClose = true }
                        },
                        c.Resolve<ILogger>(),
                        c.Resolve<ITokenStorage>(),
                        c.Resolve<IApiAppVersion>(),
                        c.Resolve<Common.Configuration.Config>().ApiVersion,
                        TranslationSource.Instance.CurrentCulture.Name);
                })
                .As<IApiClient>()
                .SingleInstance();

            builder.Register(c => new NLogLoggingConfiguration(c.Resolve<Common.Configuration.Config>().AppLogFolder, "app"))
                .AsSelf().SingleInstance();
            builder.RegisterType<NLogLoggerFactory>().As<ILoggerFactory>().SingleInstance();
            builder.Register(c => c.Resolve<ILoggerFactory>().Logger())
                .As<ILogger>().SingleInstance();
            builder.RegisterType<LogCleaner>().SingleInstance();
            builder.RegisterType<UpdateService>().AsSelf().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<ServiceClient>().SingleInstance();
            builder.RegisterType<UpdateEvents>().SingleInstance();

            builder.RegisterType<UserValidator>().SingleInstance();

            builder.Register(c => new UserAuth(
                c.Resolve<IApiClient>(),
                c.Resolve<ILogger>(),
                c.Resolve<IUserStorage>(),
                c.Resolve<ITokenStorage>())).SingleInstance();

            builder.RegisterType<NetworkClient>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<DnsClients>().As<IDnsClients>().SingleInstance();
            builder.Register(c =>
                    new SafeDnsClient(
                        new DnsClient(
                            c.Resolve<IDnsClients>(),
                            c.Resolve<INetworkInterfaces>())))
                .As<IDnsClient>().SingleInstance();

            builder.Register(c =>
                    new CachingReportClient(
                        new ReportClient(c.Resolve<IActiveUrls>().TlsReportUrl.Uri)))
                .As<IReportClient>()
                .SingleInstance();

            builder.RegisterType<EventClient>().SingleInstance();
            builder.RegisterType<UserInfoHandler>().AsImplementedInterfaces().SingleInstance();

            builder.RegisterType<VpnProfileHandler>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<VpnCredentialProvider>().SingleInstance();
            builder.Register(c => new EventTimer(
                    c.Resolve<EventClient>(),
                    c.Resolve<Common.Configuration.Config>().EventCheckInterval.RandomizedWithDeviation(0.2))) 
                .AsSelf()
                .AsImplementedInterfaces()
                .SingleInstance();
            builder.Register(c => new SafeSystemProxy(c.Resolve<ILogger>(), new SystemProxy()))
                .AsImplementedInterfaces()
                .SingleInstance();
            builder.Register(c => new MainHostname(c.Resolve<Common.Configuration.Config>().Urls.ApiUrl)).SingleInstance();
            builder.Register(c => new DohClients(
                c.Resolve<Common.Configuration.Config>().DoHProviders,
                c.Resolve<Common.Configuration.Config>().DohClientTimeout))
                .SingleInstance();
            builder.RegisterType<UnhandledExceptionLogging>().SingleInstance();
        }
    }
}
