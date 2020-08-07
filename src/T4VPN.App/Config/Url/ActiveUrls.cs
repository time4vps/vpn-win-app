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

using T4VPN.Common.Configuration;
using T4VPN.Common.OS.Processes;

namespace T4VPN.Config.Url
{
    internal class ActiveUrls : IActiveUrls
    {
        private readonly UrlConfig _config;
        private readonly IOsProcesses _processes;

        public ActiveUrls(Common.Configuration.Config config, IOsProcesses processes)
        {
            _config = config.Urls;
            _processes = processes;
        }

        public IActiveUrl PasswordResetUrl                        => Url(_config.PasswordResetUrl);
        public IActiveUrl ForgetUsernameUrl                       => Url(_config.ForgetUsernameUrl);
        public IActiveUrl UpdateUrl                               => Url(_config.UpdateUrl);
        public IActiveUrl DownloadUrl                             => Url(_config.DownloadUrl);
        public IActiveUrl ApiUrl                                  => Url(_config.ApiUrl);
        public IActiveUrl TlsReportUrl                            => Url(_config.TlsReportUrl);
        public IActiveUrl HelpUrl                                 => Url(_config.HelpUrl);
        public IActiveUrl AccountUrl                              => Url(_config.AccountUrl);
        public IActiveUrl AboutSecureCoreUrl                      => Url(_config.AboutSecureCoreUrl);
        public IActiveUrl RegisterUrl                             => Url(_config.RegisterUrl);
        public IActiveUrl TroubleShootingUrl                      => Url(_config.TroubleShootingUrl);
        public IActiveUrl P2PStatusUrl                            => Url(_config.P2PStatusUrl);
        public IActiveUrl PublicWifiSafetyUrl                     => Url(_config.PublicWifiSafetyUrl);
        public IActiveUrl Time4VPSStatusUrl                         => Url(_config.Time4VPSStatusUrl);
        public IActiveUrl TorBrowserUrl                           => Url(_config.TorBrowserUrl);
        public IActiveUrl Time4VPSTwitterUrl                      => Url(_config.Time4VPSTwitterUrl);
        public IActiveUrl SupportFormUrl                          => Url(_config.SupportFormUrl);
        public IActiveUrl AlternativeRoutingUrl                   => Url(_config.AlternativeRoutingUrl);

        private ActiveUrl Url(string url)
        {
            return new ActiveUrl(_processes, url);
        }
    }
}
