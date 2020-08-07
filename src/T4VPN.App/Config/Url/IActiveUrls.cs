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

namespace T4VPN.Config.Url
{
    public interface IActiveUrls
    {
        IActiveUrl AccountUrl { get; }
        IActiveUrl AboutSecureCoreUrl { get; }
        IActiveUrl ApiUrl { get; }
        IActiveUrl TlsReportUrl { get; }
        IActiveUrl ForgetUsernameUrl { get; }
        IActiveUrl HelpUrl { get; }
        IActiveUrl P2PStatusUrl { get; }
        IActiveUrl PasswordResetUrl { get; }
        IActiveUrl RegisterUrl { get; }
        IActiveUrl TroubleShootingUrl { get; }
        IActiveUrl UpdateUrl { get; }
        IActiveUrl DownloadUrl { get; }
        IActiveUrl PublicWifiSafetyUrl { get; }
        IActiveUrl Time4VPSStatusUrl { get; }
        IActiveUrl TorBrowserUrl { get; }
        IActiveUrl Time4VPSTwitterUrl { get; }
        IActiveUrl SupportFormUrl { get; }
        IActiveUrl AlternativeRoutingUrl { get; }
    }
}
