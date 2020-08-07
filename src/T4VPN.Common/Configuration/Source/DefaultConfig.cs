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

using Sentry.Protocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using T4VPN.Common.Configuration.Api.Handlers.TlsPinning;
using T4VPN.Common.Extensions;

namespace T4VPN.Common.Configuration.Source
{
    internal class DefaultConfig : IConfigSource
    {
        public Config Value()
        {
            var location = Assembly.GetEntryAssembly()?.Location;
            var baseFolder = (location != null ? new FileInfo(location).DirectoryName : null)
                             ?? AppDomain.CurrentDomain.BaseDirectory;

            var localAppDataFolder =
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "T4VPN");
            var commonAppDataFolder =
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "T4VPN");
            var osBits = Environment.Is64BitOperatingSystem ? 64 : 32;

            return new Config
            {
                OsBits = osBits,

                AppName = "Time4VPS VPN",

                AppVersion = Assembly.GetExecutingAssembly().GetName().Version.Normalized().ToString(),

                AppExePath = Path.Combine(baseFolder, "T4VPN.exe"),

                AppLogFolder = Path.Combine(localAppDataFolder, "Logs"),

                TranslationsFolder = baseFolder,

                ErrorMessageExePath = Path.Combine(baseFolder, "T4VPN.ErrorMessage.exe"),

                ServiceName = "Time4VPS VPN Service",

                ServiceExePath = Path.Combine(baseFolder, "T4VPNService.exe"),

                ServiceSettingsFilePath = Path.Combine(commonAppDataFolder, "ServiceSettings.json"),

                ServersJsonCacheFilePath = Path.Combine(localAppDataFolder, "Servers.json"),

                GuestHoleServersJsonFilePath = Path.Combine(localAppDataFolder, "GuestHoleServers.json"),

                ServiceLogFolder = Path.Combine(commonAppDataFolder, "Logs"),

                UpdateServiceName = "Time4VPS VPN Update Service",

                UpdateServiceExePath = Path.Combine(baseFolder, "T4VPN.UpdateService.exe"),

                UpdateServiceLogFolder = Path.Combine(commonAppDataFolder, "UpdaterLogs"),

                UpdateFilePath = Path.Combine(commonAppDataFolder, "Updates", "update.txt"),

                UpdatesPath = Path.Combine(commonAppDataFolder, "Updates"),

                SplitTunnelServiceName = "Time4VPS VPN SplitTunnel",

                LocalAppDataFolder = localAppDataFolder,

                MaxAppLogsAttached = 3,
                
                MaxServiceLogsAttached = 3,

                MaxUpdaterServiceLogsAttached = 3,

                ApiClientId = "WindowsVPN",

                ApiVersion = "1",

                UserAgent = "Time4VPS VPN",

                ApiTimeout = TimeSpan.FromSeconds(20),

                DohClientTimeout = TimeSpan.FromSeconds(10),

                ApiRetries = 0,

                MaxGuestHoleRetries = 5,

                GuestHoleVpnUsername = "guest",

                GuestHoleVpnPassword = "guest",

                UpdateFirstCheckDelay = TimeSpan.FromSeconds(15),

                UpdateCheckInterval = TimeSpan.FromHours(3),

                UpdateRemindInterval = TimeSpan.FromHours(24),

                ServerUpdateInterval = TimeSpan.FromMinutes(10),

                P2PCheckInterval = TimeSpan.FromSeconds(30),

                VpnInfoCheckInterval = TimeSpan.FromMinutes(5),

                DefaultCurrency = "USD",

                ReportBugMaxFiles = 10,

                ReportBugMaxFileSize = 488 * 1024,

                MaxProfileNameLength = 25,

                ProfileSyncTimerPeriod = TimeSpan.FromSeconds(20),

                ProfileSyncPeriod = TimeSpan.FromMinutes(5),

                ForcedProfileSyncInterval = TimeSpan.FromMinutes(3),

                EventCheckInterval = TimeSpan.FromMinutes(5),

                ServiceCheckInterval = TimeSpan.FromSeconds(30),

                DefaultOpenVpnUdpPorts = new[] { 1194 },

                DefaultOpenVpnTcpPorts = new[] { 443 },

                // Naudojamas P2P detection'e. Mes kolkas to nesupportinam
                DefaultBlackHoleIps = new List<string> { },

                Urls =
                {
                    PasswordResetUrl = "https://www.time4vps.com/knowledgebase/how-to-reset-vpn-password/",

                    ForgetUsernameUrl = "https://www.time4vps.com/knowledgebase/how-to-know-your-vpn-client-login-details/",

                    ApiUrl = "https://localhost:4200",
                    UpdateUrl = "https://localhost:4200/updates/win",
                    TlsReportUrl = "https://localhost:4200/reports/tls",

                    DownloadUrl = "https://billing.time4vps.com/downloads",                    

                    HelpUrl = "https://www.time4vps.com/kb-category/vpn/",

                    AccountUrl = "https://billing.time4vps.com/clientarea/services/secure-vpn/",

                    // Secure Core not supported
                    AboutSecureCoreUrl = "https://billing.time4vps.com/support/secure-core-vpn",

                    RegisterUrl = "https://billing.time4vps.com/signup",

                    TroubleShootingUrl = "https://www.time4vps.com/knowledgebase/ways-to-solve-time4vps-vpn-connectivity-issues-on-windows/",

                    // P2P not supported
                    P2PStatusUrl = "https://billing.time4vps.com/status",

                    PublicWifiSafetyUrl = "https://www.time4vps.com/knowledgebase/what-is-vpn/",

                    Time4VPSStatusUrl = "https://billing.time4vps.com/status",

                    TorBrowserUrl = "https://www.torproject.org",

                    Time4VPSTwitterUrl = "https://twitter.com/time4vps",

                    SupportFormUrl = "https://billing.time4vps.com/tickets/new/",
                    
                    AlternativeRoutingUrl = "https://www.time4vps.com/knowledgebase",
                },

                OpenVpn =
                {
                    ExePath = Path.Combine(baseFolder, "Resources", $"{osBits}-bit", "openvpn.exe"),

                    ConfigPath = Path.Combine(baseFolder, "Resources", "config.ovpn"),

                    ManagementHost = "127.0.0.1",

                    ExitEventName = "Time4VPS-VPN-Exit-Event",

                    OpenVpnStaticKey = ("bd00af1c45db64180de83c6c7968f0efd4a42b502c6fadddd03f4217f660440b" +
                                        "698b0adaf62f24b9b8730d5f654d12ec0b0847511e778e92b9f6f28f831ed216")
                                        .HexStringToByteArray().Skip(192).Take(64).ToArray(),

                    TlsVerifyExePath = Path.Combine(baseFolder, "T4VPN.TlsVerify.exe"),

                    TlsExportCertFolder = Path.Combine(commonAppDataFolder, "ExportCert"),

                    TapAdapterDescription = "TAP-Time4VPS VPN Windows Provider V9",

                     TapAdapterId = "tapt4vpsvpn",
                },

                TlsPinningConfig = {                
                    PinnedDomains = new List<TlsPinnedDomain>
                    {
                        new TlsPinnedDomain
                        {
                            Name = "*",
                            PublicKeyHashes = new HashSet<string>
                            {
                                "EolVhaIsoEY1mUExrXazczQeSmDKKclNGO30WbbYR3w=",
                                "Q4afFGHuDe139x0kAOkUI03GE2pXlYZKIvvcS/Npixo="
                            },
                            Enforce = true,
                            SendReport = true,
                        },
                    },
                    Enforce = true,
                },

                DoHProviders = new List<string>
                {
                    "https://dns11.quad9.net/dns-query",
                    "https://dns.google/dns-query",
                },
            };
        }
    }
}
