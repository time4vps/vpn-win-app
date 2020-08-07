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
using System.ComponentModel;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using T4VPN.Common;
using T4VPN.Common.Helpers;
using T4VPN.Common.Logging;
using T4VPN.Common.Threading;
using T4VPN.Common.Vpn;
using T4VPN.Vpn.Common;
using T4VPN.Vpn.Management;
using T4VPN.Vpn.OpenVpn;

namespace T4VPN.Vpn.Connection
{
    /// <summary>
    /// VPN connection based on OpenVPN.
    /// </summary>
    internal class OpenVpnConnection : ISingleVpnConnection
    {
        private static readonly TimeSpan WaitForConnectionTaskToFinishAfterClose = TimeSpan.FromSeconds(3);
        private static readonly TimeSpan WaitForConnectionTaskToFinishAfterCancellation = TimeSpan.FromSeconds(3);
        private const int ManagementPasswordLength = 16;

        private readonly ILogger _logger;
        private readonly ManagementClient _managementClient;
        private readonly OpenVpnProcess _process;

        private readonly OpenVpnManagementPorts _managementPorts;
        private readonly RandomStrings _randomStrings;
        private readonly SingleAction _connectAction;
        private readonly SingleAction _disconnectAction;

        private VpnEndpoint _endpoint;
        private VpnCredentials _credentials;
        private VpnError _disconnectError = VpnError.Unknown;
        private VpnConfig _config;

        public OpenVpnConnection(
            ILogger logger,
            OpenVpnProcess process,
            ManagementClient managementClient)
        {
            _logger = logger;
            _process = process;
            _managementClient = managementClient;

            _managementClient.VpnStateChanged += ManagementClient_StateChanged;
            _managementClient.TransportStatsChanged += ManagementClient_TransportStatsChanged;

            _managementPorts = new OpenVpnManagementPorts();
            _randomStrings = new RandomStrings();
            _connectAction = new SingleAction(ConnectAction);
            _connectAction.Completed += ConnectAction_Completed;
            _disconnectAction = new SingleAction(DisconnectAction);
            _disconnectAction.Completed += DisconnectAction_Completed;
        }

        public event EventHandler<EventArgs<VpnState>> StateChanged;

        public InOutBytes Total { get; private set; } = InOutBytes.Zero;

        public void Connect(VpnEndpoint endpoint, VpnCredentials credentials, VpnConfig config)
        {
            _config = config;
            _endpoint = endpoint;
            _credentials = credentials;

            _connectAction.Run();
        }

        public void Disconnect(VpnError error)
        {
            _disconnectError = error;
            _disconnectAction.Run();
        }

        private async Task ConnectAction(CancellationToken cancellationToken)
        {
            _logger.Info("OpenVpnConnection: Connect action started");

            OnStateChanged(VpnStatus.Connecting);

            var port = _managementPorts.Port();
            var password = ManagementPassword();
            var processParams = new OpenVpnProcessParams(_endpoint, port, password, _config);

            cancellationToken.ThrowIfCancellationRequested();

            if (!await _process.Start(processParams))
            {
                _disconnectError = VpnError.Unknown;
            }
            else
            {
                await _managementClient.Connect(port, password);

                if (cancellationToken.IsCancellationRequested)
                {
                    await _managementClient.CloseVpnConnection();
                    cancellationToken.ThrowIfCancellationRequested();
                }

                await _managementClient.StartVpnConnection(_credentials, cancellationToken);
            }

            cancellationToken.ThrowIfCancellationRequested();
        }

        private string ManagementPassword()
        {
            return _randomStrings.RandomString(ManagementPasswordLength);
        }

        private async Task DisconnectAction()
        {
            _logger.Info("OpenVpnConnection: Disconnect action started");
            OnStateChanged(VpnStatus.Disconnecting);

            await CloseVpnConnection();
            _managementClient.Disconnect();
            _process.Stop();
        }

        private void ConnectAction_Completed(object sender, TaskCompletedEventArgs e)
        {
            _logger.Info("OpenVpnConnection: Connect action completed");

            HandleTaskExceptionsIfAny(e.Task, "OpenVpnConnection: Connection action failed");

            if (!e.Task.IsCanceled && !_disconnectAction.IsRunning)
            {
                OnStateChanged(VpnStatus.Disconnecting);
            }
        }

        private void DisconnectAction_Completed(object sender, TaskCompletedEventArgs e)
        {
            _logger.Info("OpenVpnConnection: Disconnect action completed");

            HandleTaskExceptionsIfAny(e.Task, "OpenVpnConnection: Disconnect action failed");

            OnStateChanged(VpnStatus.Disconnected);
        }

        private async Task CloseVpnConnection()
        {
            var connectTask = _connectAction.Task;
            if (!connectTask.IsCompleted)
            {
                await TryCloseVpnConnectionAndWait(connectTask);
            }
            if (!connectTask.IsCompleted)
            {
                await CancelVpnConnectionAndWait(connectTask);
            }
        }

        private async Task TryCloseVpnConnectionAndWait(Task connectTask)
        {
            try
            {
                await _managementClient.CloseVpnConnection();
            }
            catch (Exception ex) when (IsImplementationException(ex))
            {
                _logger.Warn($"OpenVpnConnection: Failed writing to management channel: {ex.Message}");
            }

            try
            {
                _logger.Info($"OpenVpnConnection: Waiting for Connection task to finish...");
                if (await Task.WhenAny(connectTask, Task.Delay(WaitForConnectionTaskToFinishAfterClose)) != connectTask)
                {
                    _logger.Warn($"OpenVpnConnection: Connection task has not finished in {WaitForConnectionTaskToFinishAfterClose}");
                    return;
                }

                // Task completed within timeout. The task may have faulted or been canceled.
                // Re-await the task so that any exceptions/cancellation is rethrown.
                await connectTask;
            }
            catch (Exception ex) when (IsImplementationException(ex))
            {
                _logger.Error($"OpenVpnConnection: Connection task failed with exception: {ex}");
            }
        }

        private async Task CancelVpnConnectionAndWait(Task connectTask)
        {
            try
            {
                _logger.Info($"OpenVpnConnection: Cancelling Connection task");
                _connectAction.Cancel();

                _logger.Info($"OpenVpnConnection: Waiting for Connection task to finish...");
                if (await Task.WhenAny(connectTask, Task.Delay(WaitForConnectionTaskToFinishAfterCancellation)) != connectTask)
                    _logger.Warn($"OpenVpnConnection: Connection task has not finished in {WaitForConnectionTaskToFinishAfterCancellation}");
            }
            catch (Exception ex) when (IsImplementationException(ex))
            {
                _logger.Error($"OpenVpnConnection: Connection task failed: {ex}");
            }
        }

        private void ManagementClient_StateChanged(object sender, EventArgs<VpnState> e)
        {
            _logger.Info($"ManagementClient: State changed to {e.Data.Status}");

            var state = e.Data;
            if ((state.Status == VpnStatus.Connecting || state.Status == VpnStatus.Reconnecting) &&
                string.IsNullOrEmpty(state.RemoteIp))
            {
                state = new VpnState(state.Status, VpnError.None, string.Empty, _endpoint.Server.Ip, _endpoint.Protocol);
            }

            if (state.Status == VpnStatus.Disconnecting && !_disconnectAction.IsRunning)
            {
                _disconnectError = state.Error;
            }

            OnStateChanged(state);
        }

        private void ManagementClient_TransportStatsChanged(object sender, EventArgs<InOutBytes> e)
        {
            Total = e.Data;
        }

        private void OnStateChanged(VpnStatus status)
        {
            VpnState state;
            switch (status)
            {
                case VpnStatus.Connecting:
                    state = new VpnState(status, VpnError.None, string.Empty, _endpoint.Server.Ip, _endpoint.Protocol);
                    break;
                case VpnStatus.Disconnecting:
                case VpnStatus.Disconnected:
                    state = new VpnState(status, _disconnectError);
                    break;
                default:
                    state = new VpnState(status);
                    break;
            }

            _logger.Info($"OpenVpnConnection: State changed to {state.Status}, Error: {state.Error}");
            OnStateChanged(state);
        }

        private void OnStateChanged(VpnState state)
        {
            StateChanged?.Invoke(this, new EventArgs<VpnState>(state));
        }

        private void HandleTaskExceptionsIfAny(Task task, string message)
        {
            if (task.IsFaulted)
            {
                var ex = task.Exception?.InnerException;
                if (IsImplementationException(ex))
                {
                    _logger.Error(ex);
                }
                else
                {
                    throw new VpnException(message, ex);
                }
            }
        }

        private static bool IsImplementationException(Exception ex) =>
            ex is SocketException ||
            ex is Win32Exception ||
            ex is IOException;
    }
}
