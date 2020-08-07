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
using System.Threading.Tasks;
using T4VPN.Common;
using T4VPN.Common.Configuration;
using T4VPN.Common.Extensions;
using T4VPN.Common.Logging;
using T4VPN.Common.OS.Processes;
using T4VPN.Vpn.OpenVpn.Arguments;

namespace T4VPN.Vpn.OpenVpn
{
    /// <summary>
    /// Starts and stops OpenVPN process.
    /// </summary>
    public class OpenVpnProcess
    {
        private static readonly TimeSpan WaitAfterSignalingExit = TimeSpan.FromSeconds(6);

        private readonly ILogger _logger;
        private readonly IOsProcesses _processes;
        private readonly OpenVpnExitEvent _processExitEvent;
        private readonly OpenVpnConfig _config;
        private readonly IOsProcess _nullProcess;

        private IOsProcess _process;
        private TaskCompletionSource<bool> _startCompletionSource;

        internal OpenVpnProcess(
                ILogger logger,
                IOsProcesses processes,
                OpenVpnExitEvent processExitEvent,
                OpenVpnConfig config)
        {
            _logger = logger;
            _processes = processes;
            _processExitEvent = processExitEvent;
            _config = config;

            _process =_nullProcess = new NullOsProcess();
        }

        public Task<bool> Start(OpenVpnProcessParams processParams)
        {
            var localIp = new BestLocalEndpoint(processParams.Endpoint).Ip();

            var arguments = new CommandLineArguments()
                .Add(new BasicArguments(_config))
                .Add(new ManagementArguments(_config, processParams.ManagementPort))
                .Add(new EndpointArguments(processParams.Endpoint))
                .Add(new BindArguments(localIp))
                .Add(new CustomDnsArguments(processParams.CustomDns))
                .Add(new TlsVerifyArguments(_config, processParams.Endpoint.Server.Name));

            _startCompletionSource?.TrySetCanceled();
            _startCompletionSource = new TaskCompletionSource<bool>();

            _process = _processes.Process(_config.ExePath, arguments);
            AddEventHandlers();
            _process.Start();

            _logger.Info("OpenVPN <- Management channel password");
            _process.StandardInput.WriteLine(processParams.Password);

            return _startCompletionSource.Task;
        }

        public void Stop()
        {
            _startCompletionSource?.TrySetCanceled();
            SignalProcessToExit();
            WaitForProcessToExit(WaitAfterSignalingExit);
            KillNotExitedProcesses();
            Cleanup();
        }

        private void AddEventHandlers()
        {
            _process.OutputDataReceived += Process_OutputDataReceived;
            _process.ErrorDataReceived += Process_ErrorDataReceived;
            _process.Exited += Process_Exited;
        }

        private void SignalProcessToExit()
        {
            _processExitEvent.Signal();
        }

        private void WaitForProcessToExit(TimeSpan duration)
        {
            _process.WaitForExit(duration);
        }

        private void KillNotExitedProcesses()
        {
            var processes = _processes.ProcessesByPath(_config.ExePath);
            foreach (var process in processes)
            {
                process.Kill();
                process.Dispose();
            }
        }

        private void Process_OutputDataReceived(object sender, EventArgs<string> e)
        {
            _logger.Info($"OpenVPN -> {e.Data}");

            if (e.Data.StartsWithIgnoringCase("MANAGEMENT: TCP Socket listening on"))
            {
                _startCompletionSource.TrySetResult(true);
            }
            else if (e.Data == null)
            {
                _startCompletionSource.TrySetResult(false);
            }
        }

        private void Process_ErrorDataReceived(object sender, EventArgs<string> e)
        {
            var message = $"OpenVPN -> {e.Data}";

            if (e.Data.StartsWithIgnoringCase("Enter Management Password:"))
            {
                _logger.Info(message);
            }
            else
            {
                _logger.Warn(message);
            }
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            _logger.Info("OpenVPN process exited");

            _startCompletionSource.TrySetResult(false);
        }

        private void Cleanup()
        {
            _process?.Dispose();
            _process = _nullProcess;
        }
    }
}
