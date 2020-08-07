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

using T4VPN.Common.Logging;
using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace T4VPN.Vpn.Management
{
    /// <summary>
    /// TCP connection to OpenVPN management interface.
    /// </summary>
    internal class TcpManagementChannel : IManagementChannel
    {
        private readonly ILogger _logger;
        private readonly string _host;
        private StreamReader _streamReader;
        private StreamWriter _streamWriter;
        private TcpClient _tcpClient;

        public TcpManagementChannel(ILogger logger, string host)
        {
            _logger = logger;
            _host = host;
        }

        public async Task Connect(int port)
        {
            Disconnect();

            var tcpClient = new TcpClient();

            _logger.Info($"Connecting to OpenVPN management interface on {_host}:{port}");
            await tcpClient.ConnectAsync(_host, port);

            _streamReader = new StreamReader(tcpClient.GetStream());
            _streamWriter = new StreamWriter(tcpClient.GetStream())
            {
                AutoFlush = true
            };
            _tcpClient = tcpClient;
        }

        public async Task WriteLine(string message)
        {
            try
            {
                await WriteLineInternal(message);
            }
            catch (ObjectDisposedException ex)
            {
                throw new IOException("The NetworkStream is closed", ex);
            }
        }

        public async Task<string> ReadLine()
        {
            try
            {
                return await ReadLineInternal();
            }
            catch (ObjectDisposedException ex)
            {
                throw new IOException("The NetworkStream is closed", ex);
            }
        }

        public void Disconnect()
        {
            if (_tcpClient != null)
            {
                if (_tcpClient.Connected)
                {
                    _logger.Info("Disconnecting from OpenVPN management interface");
                }
                _tcpClient.Close();
                SafeDispose(ref _tcpClient);
            }

            SafeDispose(ref _streamReader);
            SafeDispose(ref _streamWriter);
        }

        private Task WriteLineInternal(string message)
        {
            var writer = _streamWriter;
            if (writer == null)
                throw new IOException("OpenVPN management interface is not connected");

            return writer.WriteLineAsync(message);
        }

        private async Task<string> ReadLineInternal()
        {
            var reader = _streamReader;
            if (reader == null)
                throw new IOException("OpenVPN management interface is not connected");
            var message = await reader.ReadLineAsync();

            if (message == null)
            {
                _logger.Info("Disconnected from OpenVPN management interface");
            }

            return message;
        }

        private void SafeDispose<T>(ref T disposable) where T: class, IDisposable
        {
            var item = disposable;
            disposable = null;
            try
            {
                item?.Dispose();
            }
            catch (InvalidOperationException ex)
            {
                _logger.Error($"Failed to disconnect from OpenVPN management interface: {ex.Message}");
            }
        }
    }
}
