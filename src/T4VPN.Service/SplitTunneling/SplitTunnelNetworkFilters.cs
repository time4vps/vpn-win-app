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
using System.Net;
using T4VPN.NetworkFilter;

namespace T4VPN.Service.SplitTunneling
{
    public class SplitTunnelNetworkFilters
    {
        private const uint WfpSubLayerWeight = 7;
        private static readonly Guid WfpCalloutKey = Guid.Parse("{3c5a284f-af01-51fa-4361-6c6c50424144}");
        private static readonly Guid WfpRedirectUDPCalloutKey = Guid.Parse("{10636af3-50d6-4f53-acb7-d5af33217fca}");

        private IpFilter _ipFilter;
        private Sublayer _subLayer;

        public void EnableExcludeMode(string[] apps, string[] ips, IPAddress internetLocalIp)
        {
            Create();

            _ipFilter.Session.StartTransaction();
            try
            {
                var connectRedirectCallout = CreateConnectRedirectCallout();
                var redirectUDPCallout = CreateUDPRedirectCallout();

                var providerContext = _ipFilter.CreateProviderContext(
                    new DisplayData
                    {
                        Name = "Time4VPS VPN Split Tunnel redirect context",
                        Description = "Instructs the callout driver where to redirect network connections",
                    },
                    new ConnectRedirectData(internetLocalIp));

                CreateAppFilters(apps, redirectUDPCallout, Layer.BindRedirectV4, providerContext);                

                CreateAppFilters(apps, connectRedirectCallout, Layer.AppConnectRedirectV4, providerContext);
                CreateIPFilters(ips, connectRedirectCallout, Layer.AppConnectRedirectV4, providerContext);

                _ipFilter.Session.CommitTransaction();
            }
            catch
            {
                _ipFilter.Session.AbortTransaction();
                throw;
            }
        }

        public void EnableIncludeMode(string[] apps, string[] ips, IPAddress internetLocalIp, IPAddress vpnLocalIp)
        {
            Create();

            _ipFilter.Session.StartTransaction();
            try
            {
                var connectRedirectCallout = CreateConnectRedirectCallout();
                var redirectUDPCallout = CreateUDPRedirectCallout();

                var providerContext = _ipFilter.CreateProviderContext(
                    new DisplayData
                    {
                        Name = "Time4VPS VPN Split Tunnel redirect context",
                        Description = "Instructs the callout driver where to redirect network connections",
                    },
                    new ConnectRedirectData(internetLocalIp));

                _subLayer.CreateLayerCalloutFilter(
                        new DisplayData
                        {
                            Name = "Time4VPS VPN Split Tunnel redirect",
                            Description = "Redirects network connections"
                        },
                        Layer.AppConnectRedirectV4,
                        14,
                        connectRedirectCallout,
                        providerContext);

                _subLayer.CreateLayerCalloutFilter(
                        new DisplayData
                        {
                            Name = "Time4VPS VPN Split Tunnel redirect",
                            Description = "Redirects network connections"
                        },
                        Layer.BindRedirectV4,
                        14,
                        redirectUDPCallout,
                        providerContext);

                providerContext = _ipFilter.CreateProviderContext(
                    new DisplayData
                    {
                        Name = "Time4VPS VPN Split Tunnel redirect context",
                        Description = "Instructs the callout driver where to redirect network connections",
                    },
                    new ConnectRedirectData(vpnLocalIp));

                CreateAppFilters(apps, connectRedirectCallout, Layer.AppConnectRedirectV4, providerContext);
                CreateIPFilters(ips, connectRedirectCallout, Layer.AppConnectRedirectV4, providerContext);

                CreateAppFilters(apps, redirectUDPCallout, Layer.BindRedirectV4, providerContext);

                _ipFilter.Session.CommitTransaction();
            }
            catch
            {
                _ipFilter.Session.AbortTransaction();
                throw;
            }
        }

        public void Disable()
        {
            Remove();
        }

        private void Create()
        {
            _ipFilter = IpFilter.Create(
                Session.Dynamic(),
                new DisplayData { Name = "Time4VPS VPN Split Tunnel redirect app", Description = "Time4VPS VPN Split Tunnel provider" });

            _subLayer = _ipFilter.CreateSublayer(
                new DisplayData { Name = "Time4VPS VPN Split Tunnel filters" },
                WfpSubLayerWeight);
        }

        private void Remove()
        {
            _ipFilter?.Session.Close();
            _ipFilter = null;
            _subLayer = null;
        }

        private void CreateAppFilters(string[] apps, Callout callout, Layer layer, ProviderContext providerContext)
        {
            foreach (var app in apps)
            {
                SafeCreateAppFilter(app, callout, layer, providerContext);
            }
        }

        private void SafeCreateAppFilter(string app, Callout callout, Layer layer, ProviderContext providerContext)
        {
            try
            {
                CreateAppFilter(app, callout, layer, providerContext);
            }
            catch (NetworkFilterException)
            {
            }
        }

        private void CreateAppFilter(string app, Callout callout, Layer layer, ProviderContext providerContext)
        {
            _subLayer.CreateAppCalloutFilter(
                new DisplayData
                {
                    Name = "Time4VPS VPN Split Tunnel redirect app",
                    Description = "Redirects network connections of the app"
                },
                layer,
                15,
                callout,
                providerContext,
                app);
        }

        private void CreateIPFilters(string[] ips, Callout callout, Layer layer, ProviderContext providerContext)
        {
            foreach (var ip in ips)
            {
                SafeCreateIPFilter(ip, callout, layer, providerContext);
            }
        }

        private void SafeCreateIPFilter(string ip, Callout callout, Layer layer, ProviderContext providerContext)
        {
            try
            {
                CreateIPFilter(ip, callout, layer, providerContext);
            }
            catch (NetworkFilterException)
            {
            }
        }

        private void CreateIPFilter(string ip, Callout callout, Layer layer, ProviderContext providerContext)
        {
            var networkAddress = new Common.OS.Net.NetworkAddress(ip);
            if (!networkAddress.Valid())
            {
                return;
            }

            _subLayer.CreateRemoteNetworkIPv4CalloutFilter(
                new DisplayData
                {
                    Name = "Time4VPS VPN Split Tunnel redirect IP",
                    Description = "Redirects network connections of the IP"
                },
                layer,
                15,
                callout,
                providerContext,
                new NetworkAddress(networkAddress.Ip, networkAddress.Mask)
            );
        }

        private Callout CreateConnectRedirectCallout()
        {
            return _ipFilter.CreateCallout(
                new DisplayData
                {
                    Name = "Time4VPS VPN Split Tunnel callout",
                    Description = "Redirects network connections",
                },
                WfpCalloutKey,
                Layer.AppConnectRedirectV4
            );
        }

        private Callout CreateUDPRedirectCallout()
        {
            return _ipFilter.CreateCallout(
                new DisplayData
                {
                    Name = "Time4VPS VPN Split Tunnel callout",
                    Description = "Redirects UDP network flow",
                },
                WfpRedirectUDPCalloutKey,
                Layer.BindRedirectV4
            );
        }
    }
}
