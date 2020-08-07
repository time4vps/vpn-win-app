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
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using T4VPN.Common;
using T4VPN.Common.Extensions;
using T4VPN.Common.Helpers;
using T4VPN.Common.Logging;
using T4VPN.Common.Threading;
using T4VPN.Common.Vpn;
using T4VPN.Service.Contract.Settings;
using T4VPN.Service.Contract.Vpn;
using T4VPN.Service.Settings;
using T4VPN.Vpn.Common;

namespace T4VPN.Service
{
    [ServiceBehavior(
        InstanceContextMode = InstanceContextMode.Single,
        ConcurrencyMode = ConcurrencyMode.Single)]
    public class VpnConnectionHandler : IVpnConnectionContract
    {
        private readonly object _callbackLock = new object();
        private readonly List<IVpnEventsContract> _callbacks = new List<IVpnEventsContract>();

        private readonly KillSwitch.KillSwitch _killSwitch;
        private readonly IVpnConnection _vpnConnection;
        private readonly ILogger _logger;
        private readonly IServiceSettings _serviceSettings;
        private readonly ITaskQueue _taskQueue;

        private VpnState _state = new VpnState(VpnStatus.Disconnected);

        public VpnConnectionHandler(
            KillSwitch.KillSwitch killSwitch,
            IVpnConnection vpnConnection,
            ILogger logger,
            IServiceSettings serviceSettings,
            ITaskQueue taskQueue)
        {
            _killSwitch = killSwitch;
            _vpnConnection = vpnConnection;
            _logger = logger;
            _serviceSettings = serviceSettings;
            _taskQueue = taskQueue;
            _vpnConnection.StateChanged += VpnConnection_StateChanged;
        }

        public Task Connect(VpnConnectionRequestContract connectionRequest)
        {
            Ensure.NotNull(connectionRequest, nameof(connectionRequest));

            _logger.Info("Connect requested");

            _serviceSettings.Apply(connectionRequest.Settings);

            var endpoints = Map(connectionRequest.Servers);
            var protocol = Map(connectionRequest.Protocol);
            var credentials = Map(connectionRequest.Credentials);
            var config = Map(connectionRequest.VpnConfig);

            _vpnConnection.Connect(
                endpoints,
                config,
                protocol,
                credentials);

            return Task.CompletedTask;
        }

        public Task UpdateServers(VpnHostContract[] servers, VpnConfigContract config)
        {
            Ensure.NotNull(servers, nameof(servers));
            Ensure.NotNull(config, nameof(config));

            _logger.Info("Update Servers requested");

            _vpnConnection.UpdateServers(
                Map(servers),
                Map(config));

            return Task.CompletedTask;
        }

        public Task Disconnect(SettingsContract settings, VpnErrorTypeContract vpnError)
        {
            _logger.Info("Disconnect requested");

            _serviceSettings.Apply(settings);

            _vpnConnection.Disconnect(Map(vpnError));

            return Task.CompletedTask;
        }

        public Task RepeatState()
        {
            _taskQueue.Enqueue(() =>
            {
                CallbackStateChanged(_state);
            });

            return Task.CompletedTask;
        }

        public Task<InOutBytesContract> Total()
        {
            return Map(_vpnConnection.Total).AsTask();
        }

        public Task RegisterCallback()
        {
            lock (_callbackLock)
            {
                _callbacks.Add(OperationContext.Current.GetCallbackChannel<IVpnEventsContract>());
            }

            return Task.CompletedTask;
        }

        public Task UnRegisterCallback()
        {
            _logger.Info("Unregister callback requested");

            lock (_callbackLock)
            {
                _callbacks.Remove(OperationContext.Current.GetCallbackChannel<IVpnEventsContract>());
            }

            return Task.CompletedTask;
        }

        private void VpnConnection_StateChanged(object sender, EventArgs<VpnState> e)
        {
            _state = e.Data;
            CallbackStateChanged(_state);
        }

        private void CallbackStateChanged(VpnState state)
        {
            _logger.Info($"Callbacking VPN state {state.Status}");
            Callback(callback => callback.OnStateChanged(Map(state)));
        }

        private void Callback(Action<IVpnEventsContract> action)
        {
            lock (_callbackLock)
            {
                foreach (var callback in _callbacks.ToList())
                {
                    try
                    {
                        action(callback);
                    }
                    catch (Exception ex) when (ex.IsServiceCommunicationException())
                    {
                        _logger.Warn($"Callback failed: {ex.Message}");
                        _callbacks.Remove(callback);
                    }
                    catch (TimeoutException)
                    {
                        _logger.Warn("Callback timed out");
                    }
                }
            }
        }

        private VpnStateContract Map(VpnState state)
        {
            return new VpnStateContract(
                Map(state.Status),
                Map(state.Error),
                state.RemoteIp,
                _killSwitch.ExpectedLeakProtectionStatus(state),
                Map(state.Protocol));
        }

        private static VpnStatusContract Map(VpnStatus vpnStatus)
        {
            return (VpnStatusContract) vpnStatus;
        }

        private static VpnProtocolContract Map(VpnProtocol protocol)
        {
            return (VpnProtocolContract)protocol;
        }

        private static VpnProtocol Map(VpnProtocolContract protocol)
        {
            return (VpnProtocol) protocol;
        }

        private static VpnCredentials Map(VpnCredentialsContract credentials)
        {
            return new VpnCredentials(
                credentials.Username,
                credentials.Password
            );
        }

        private static IReadOnlyList<VpnHost> Map(IEnumerable<VpnHostContract> servers)
        {
            return servers.Select(Map).ToList();
        }

        private static VpnHost Map(VpnHostContract server)
        {
            return new VpnHost(server.Name, server.Ip);
        }

        private static VpnConfig Map(VpnConfigContract config)
        {
            var portConfig = config.Ports.ToDictionary(p => Map(p.Key), p => (IReadOnlyCollection<int>)p.Value.ToList());
            return new VpnConfig(portConfig, config.CustomDns);
        }

        private static InOutBytesContract Map(InOutBytes bytes)
        {
            return new InOutBytesContract(bytes.BytesIn, bytes.BytesOut);
        }

        private static VpnError Map(VpnErrorTypeContract errorType)
        {
            return (VpnError)errorType;
        }

        private static VpnErrorTypeContract Map(VpnError errorType)
        {
            return (VpnErrorTypeContract) errorType;
        }
    }
}
