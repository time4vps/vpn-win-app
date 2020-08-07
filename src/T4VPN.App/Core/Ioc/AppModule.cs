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
using System.Windows;
using Autofac;
using Caliburn.Micro;
using T4VPN.About;
using T4VPN.Account;
using T4VPN.Common.Configuration;
using T4VPN.Common.Logging;
using T4VPN.Common.OS.Services;
using T4VPN.Common.Storage;
using T4VPN.Common.Text.Serialization;
using T4VPN.Config;
using T4VPN.Core.Api;
using T4VPN.Core.Api.Contracts;
using T4VPN.Core.Auth;
using T4VPN.Core.Modals;
using T4VPN.Core.Profiles;
using T4VPN.Core.Profiles.Cached;
using T4VPN.Core.Servers;
using T4VPN.Core.Servers.Contracts;
using T4VPN.Core.Service;
using T4VPN.Core.Service.Settings;
using T4VPN.Core.Service.Vpn;
using T4VPN.Core.Settings;
using T4VPN.Core.Settings.Migrations;
using T4VPN.Core.Startup;
using T4VPN.Core.Storage;
using T4VPN.Core.User;
using T4VPN.FlashNotifications;
using T4VPN.Map;
using T4VPN.Modals;
using T4VPN.Modals.Dialogs;
using T4VPN.Notifications;
using T4VPN.Servers;
using T4VPN.Settings;
using T4VPN.Settings.ReconnectNotification;
using T4VPN.Settings.SplitTunneling;
using T4VPN.Sidebar;
using T4VPN.Vpn;
using T4VPN.Vpn.Connectors;
using VpnConfig = T4VPN.Config.VpnConfig;

namespace T4VPN.Core.Ioc
{
    public class AppModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.Register(c => new ConfigFactory().Config());

            builder.RegisterType<Bootstrapper>().SingleInstance();
            builder.RegisterType<EventAggregator>().As<IEventAggregator>().SingleInstance();
            builder.RegisterType<JsonSerializerFactory>().As<ITextSerializerFactory>().SingleInstance();

            builder.RegisterType<SidebarManager>().SingleInstance();
            builder.RegisterType<PricingBuilder>().SingleInstance();
            builder.RegisterType<UpdateViewModel>().AsSelf().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<VpnConnectionSpeed>().AsImplementedInterfaces().AsSelf().SingleInstance();

            builder.Register(c => new CollectionCache<LogicalServerContract>(
                c.Resolve<ILogger>(),
                c.Resolve<ITextSerializerFactory>(),
                c.Resolve<Common.Configuration.Config>().ServersJsonCacheFilePath))
                .As<ICollectionStorage<LogicalServerContract>>()
                .SingleInstance();

            builder.Register(c => new CollectionCache<GuestHoleServerContract>(
                    c.Resolve<ILogger>(),
                    c.Resolve<ITextSerializerFactory>(),
                    c.Resolve<Common.Configuration.Config>().GuestHoleServersJsonFilePath))
                .As<ICollectionStorage<GuestHoleServerContract>>()
                .SingleInstance();

            builder.RegisterType<ApiServers>().SingleInstance();
            builder.RegisterType<ServerUpdater>().AsImplementedInterfaces().SingleInstance();

            builder.RegisterType<UserStorage>().As<IUserStorage>().SingleInstance();
            builder.RegisterType<TruncatedLocation>().SingleInstance();

            builder.RegisterType<ServerCandidatesFactory>().SingleInstance();
            builder.RegisterType<PinFactory>()
                .AsImplementedInterfaces()
                .AsSelf()
                .SingleInstance();

            builder.RegisterType<SortedCountries>().SingleInstance();
            builder.RegisterType<ServerListFactory>().AsImplementedInterfaces().AsSelf().SingleInstance();
            builder.RegisterInstance((App) Application.Current).SingleInstance();
            builder.RegisterType<VpnService>().AsSelf().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<ModalWindows>().As<IModalWindows>().SingleInstance();
            builder.RegisterType<T4VPN.Modals.Modals>().As<IModals>().SingleInstance();
            builder.RegisterType<Dialogs>().As<IDialogs>().SingleInstance();
            builder.RegisterType<AutoStartup>().As<IAutoStartup>().SingleInstance();
            builder.RegisterType<SyncableAutoStartup>().As<ISyncableAutoStartup>().SingleInstance();
            builder.RegisterType<SyncedAutoStartup>().AsSelf().As<ISettingsAware>().SingleInstance();

            builder.RegisterType<AppSettingsStorage>().SingleInstance();
            builder.Register(c => 
                    new EnumerableAsJsonSettings(
                        new CachedSettings(
                            new EnumAsStringSettings(
                                new SelfRepairingSettings(
                                    c.Resolve<AppSettingsStorage>())))))
                .As<ISettingsStorage>()
                .SingleInstance();

            builder.RegisterType<PerUserSettings>()
                .AsSelf()
                .As<ILoggedInAware>()
                .As<ILogoutAware>()
                .SingleInstance();
            builder.RegisterType<UserSettings>().SingleInstance();

            builder.RegisterType<PredefinedProfiles>().SingleInstance();
            builder.RegisterType<CachedProfiles>().SingleInstance();
            builder.RegisterType<ApiProfiles>().SingleInstance();
            builder.RegisterType<Profiles.Profiles>().SingleInstance();
            builder.RegisterType<SyncProfiles>().AsSelf().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<SyncProfile>().SingleInstance();

            builder.RegisterType<AppSettings>().AsImplementedInterfaces().SingleInstance();
            // builder.RegisterType<Settings.Migrations.Examplev1.0.1.AppSettingsMigration>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<Settings.Migrations.v1_0_3.AppSettingsMigration>().AsImplementedInterfaces().SingleInstance();

            builder.RegisterType<MapLineManager>().AsImplementedInterfaces().AsSelf().SingleInstance();
            builder.RegisterType<VpnEvents>();
            builder.RegisterType<SettingsServiceClientManager>().SingleInstance();
            builder.RegisterType<SettingsServiceClient>().SingleInstance();
            builder.RegisterType<ServiceChannelFactory>().SingleInstance();
            builder.RegisterType<SettingsContractProvider>().SingleInstance();
            builder.RegisterType<AutoConnect>().SingleInstance();
            builder.RegisterInstance(Properties.Settings.Default);

            builder.Register(c => new ServiceRetryPolicy(2, TimeSpan.FromSeconds(2))).SingleInstance();
            builder.Register(c =>
                new VpnSystemService(
                    new ReliableService(
                        c.Resolve<ServiceRetryPolicy>(),
                            new SafeService(
                                new LoggingService(
                                    c.Resolve<ILogger>(),
                                    new SystemService(
                                        c.Resolve<Common.Configuration.Config>().ServiceName))))))
                .SingleInstance();
            builder.Register(c =>
                new AppUpdateSystemService(
                    new ReliableService(
                        c.Resolve<ServiceRetryPolicy>(),
                            new SafeService(
                                new LoggingService(
                                    c.Resolve<ILogger>(),
                                    new SystemService(
                                        c.Resolve<Common.Configuration.Config>().UpdateServiceName))))))
                .SingleInstance();

            builder.RegisterType<VpnServiceManager>().SingleInstance();
            builder.Register(c => new ServiceStartDecorator(
                c.Resolve<ILogger>(),
                c.Resolve<VpnServiceManager>(),
                c.Resolve<IModals>()))
                .As<IVpnServiceManager>().SingleInstance();
            builder.RegisterType<VpnManager>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<ServerConnector>().SingleInstance();
            builder.RegisterType<ProfileConnector>().SingleInstance();
            builder.RegisterType<CountryConnector>().SingleInstance();
            builder.RegisterType<GuestHoleConnector>().AsImplementedInterfaces().AsSelf().SingleInstance();
            builder.RegisterType<GuestHoleState>().SingleInstance();
            builder.RegisterType<DisconnectError>().AsImplementedInterfaces().AsSelf().SingleInstance();
            builder.RegisterType<VpnStateNotification>()
                .AsImplementedInterfaces()
                .AsSelf()
                .SingleInstance();
            builder.RegisterType<OutdatedAppNotification>().AsImplementedInterfaces().AsSelf().SingleInstance();
            builder.RegisterType<QuickConnector>().SingleInstance();
            builder.RegisterType<AppExitHandler>().AsImplementedInterfaces().AsSelf().SingleInstance();
            builder.RegisterType<UserLocationService>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<VpnInfoChecker>().AsImplementedInterfaces().AsSelf().SingleInstance();
            builder.RegisterType<InstalledApps>().SingleInstance();
            builder.RegisterType<Onboarding.Onboarding>().AsSelf().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<SystemNotification>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<VpnConfig>().As<IVpnConfig>().SingleInstance();
            builder.RegisterType<MonitoredVpnService>().AsImplementedInterfaces().AsSelf().SingleInstance();
            builder.Register(c => new UpdateNotification(
                    c.Resolve<Common.Configuration.Config>().UpdateRemindInterval,
                    c.Resolve<ISystemNotification>(),
                    c.Resolve<UserAuth>(),
                    c.Resolve<IEventAggregator>(),
                    c.Resolve<UpdateFlashNotificationViewModel>()))
                .AsImplementedInterfaces()
                .SingleInstance();
            builder.RegisterType<SystemProxyNotification>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<InsecureNetworkNotification>().AsImplementedInterfaces().AsSelf().SingleInstance();
            builder.Register(c => new Language(
                    c.Resolve<IAppSettings>(),
                    c.Resolve<ILanguageProvider>(),
                    c.Resolve<Common.Configuration.Config>().DefaultLocale))
                .AsSelf()
                .AsImplementedInterfaces()
                .SingleInstance();
            builder.Register(c => new LanguageProvider(c.Resolve<ILogger>(),
                c.Resolve<Common.Configuration.Config>().TranslationsFolder,
                c.Resolve<Common.Configuration.Config>().DefaultLocale))
                .As<ILanguageProvider>()
                .AsSelf()
                .SingleInstance();
            builder.RegisterType<ExpiredSessionHandler>().AsImplementedInterfaces().AsSelf().SingleInstance();
            builder.RegisterType<ReconnectState>().AsImplementedInterfaces().AsSelf().SingleInstance();
            builder.RegisterType<SettingsBuilder>().SingleInstance();
        }
    }
}
