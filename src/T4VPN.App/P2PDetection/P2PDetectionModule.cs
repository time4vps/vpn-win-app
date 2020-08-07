﻿/*
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
using T4VPN.Common.OS.Net.Http;
using T4VPN.Config;
using T4VPN.Config.Url;
using T4VPN.Core.User;
using T4VPN.Core.Vpn;
using T4VPN.P2PDetection.Blocked;
using T4VPN.P2PDetection.Forwarded;
using Module = Autofac.Module;

namespace T4VPN.P2PDetection
{
    public class P2PDetectionModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.Register(c =>
                    new SafeBlockedTraffic(
                        new BlockedTraffic(
                            c.Resolve<IHttpClients>(),
                            c.Resolve<IActiveUrls>().P2PStatusUrl.Uri,
                            new P2PDetectionTimeout(c.Resolve<Common.Configuration.Config>().P2PCheckInterval))))
                .As<IBlockedTraffic>()
                .SingleInstance();

            builder.Register(c => 
                    new ForwardedTraffic(
                        c.Resolve<IUserLocationService>(),
                        c.Resolve<IVpnConfig>()))
                .AsImplementedInterfaces()
                .SingleInstance();

            builder.RegisterType<P2PDetector>()
                .As<IVpnStateAware>()
                .AsSelf()
                .SingleInstance();
        }
    }
}
