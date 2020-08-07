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
using System.Collections.Specialized;
using System.ComponentModel;
using T4VPN.Common;
using T4VPN.Core.Models;
using T4VPN.Core.Native.Structures;
using T4VPN.Core.Profiles.Cached;
using T4VPN.Core.Settings.Contracts;

namespace T4VPN.Core.Settings
{
    public interface IAppSettings
    {
        event PropertyChangedEventHandler PropertyChanged;
        CachedProfileDataContract Profiles { get; set; }
        DateTime ProfileChangesSyncedAt { get; set; }
        string OvpnProtocol { get; set; }
        string Language { get; set; }
        bool AppFirstRun { get; set; }
        bool ShowNotifications { get; set; }
        WindowPlacement WindowPlacement { get; set; }
        WindowPlacement SidebarWindowPlacement { get; set; }
        double Width { get; set; }
        string AutoConnect { get; set; }
        string QuickConnect { get; set; }
        string LastEventId { get; set; }
        bool StartOnStartup { get; set; }        
        StartMinimizedMode StartMinimized { get; set; }
        bool EarlyAccess { get; set; }        
        bool SecureCore { get; set; }
        string LastUpdate { get; set; }
        bool KillSwitch { get; set; }
        bool Ipv6LeakProtection { get; set; }
        bool CustomDnsEnabled { get; set; }
        bool SidebarMode { get; set; }
        bool WelcomeModalShown { get; set; }
        long TrialExpirationTime { get; set; }
        bool AboutToExpireModalShown { get; set; }
        bool ExpiredModalShown { get; set; }
        int OnboardingStep { get; set; }
        int AppStartCounter { get; set; }
        int SidebarTab { get; set; }
        SplitTunnelingApp[] SplitTunnelingBlockApps { get; set; }
        SplitTunnelingApp[] SplitTunnelingAllowApps { get; set; }
        IpContract[] SplitTunnelExcludeIps { get; set; }
        IpContract[] SplitTunnelIncludeIps { get; set; }
        IpContract[] CustomDnsIps { get; set; }
        int SettingsSelectedTabIndex { get; set; }
        bool SplitTunnelingEnabled { get; set; }
        SplitTunnelMode SplitTunnelMode { get; set; }
        string[] GetSplitTunnelApps();
        bool NetShieldEnabled { get; set; }
        int NetShieldMode { get; set; }
        bool NetShieldModalShown { get; set; }
        DateTime LastPrimaryApiFail { get; set; }
        StringCollection AlternativeApiBaseUrls { set; get; }
        string ActiveAlternativeApiBaseUrl { set; get; }
        bool DoHEnabled { get; set; }
    }
}
