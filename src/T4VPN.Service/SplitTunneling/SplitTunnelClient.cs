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

using System;
using System.IO;
using System.Linq;
using System.Net;
using T4VPN.Common.Logging;
using T4VPN.NetworkFilter;

namespace T4VPN.Service.SplitTunneling
{
    public class SplitTunnelClient : ISplitTunnelClient
    {
        private readonly ILogger _logger;
        private readonly BestNetworkInterface _bestInterface;
        private readonly SplitTunnelNetworkFilters _filters;

        public SplitTunnelClient(
            ILogger logger,
            BestNetworkInterface bestInterface,
            SplitTunnelNetworkFilters filters)
        {
            _logger = logger;
            _bestInterface = bestInterface;
            _filters = filters;
        }

        public void EnableExcludeMode(string[] appPaths, string[] ips)
        {
            var apps = GetAppPaths(appPaths);
            if ((apps == null || apps.Length == 0) && (ips == null || ips.Length == 0))
            {
                return;
            }

            EnsureSucceeded(
                () => _filters.EnableExcludeMode(apps, ips, _bestInterface.LocalIpAddress()),
                "SplitTunnel: Enabling exclude mode");
        }

        public void EnableIncludeMode(string[] appPaths, string[] ips, string vpnLocalIp)
        {
            var apps = GetAppPaths(appPaths);
            if ((apps == null || apps.Length == 0) && (ips == null || ips.Length == 0))
            {
                return;
            }

            EnsureSucceeded(
                () => _filters.EnableIncludeMode(
                    apps,
                    ips,
                    _bestInterface.LocalIpAddress(),
                    IPAddress.Parse(vpnLocalIp)),
                "SplitTunnel: Enabling include mode");
        }

        public void Disable()
        {
            EnsureSucceeded(
                () => _filters.Disable(),
                "SplitTunnel: Disabling");
        }

        private string[] GetAppPaths(string[] paths)
        {
            return paths.Where(File.Exists).ToArray();
        }

        private void EnsureSucceeded(System.Action action, string actionMessage)
        {
            try
            {
                action();
                _logger.Info($"{actionMessage} succeeded");
            }
            catch (NetworkFilterException e)
            {
                _logger.Error($"{actionMessage} failed. Error code: {e.Code}");
            }
        }
    }
}