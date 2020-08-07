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
using System.Threading.Tasks;

namespace T4VPN.Vpn.Management
{
    /// <summary>
    /// Messaging wrapper over <see cref="IManagementChannel"/>. 
    /// </summary>
    internal class MessagingManagementChannel
    {
        private readonly ILogger _logger;
        private readonly IManagementChannel _managementChannel;

        public MessagingManagementChannel(ILogger logger, IManagementChannel managementChannel) 
        {
            _logger = logger;
            _managementChannel = managementChannel;

            Messages = new ManagementMessages();
        }

        public ManagementMessages Messages { get; }

        public async Task Connect(int port, string password)
        {
            await _managementChannel.Connect(port);
            _logger.Info("Management <- [management password]");
            await _managementChannel.WriteLine(password);
        }

        public async Task<ReceivedManagementMessage> ReadMessage()
        {
            string messageText = await _managementChannel.ReadLine();
            var message = Messages.ReceivedMessage(messageText ?? "");
            Log(message);
            return message;
        }

        public Task WriteMessage(ManagementMessage message)
        {
            Log(message);
            return _managementChannel.WriteLine(message.ToString());
        }

        public void Disconnect()
        {
            _managementChannel.Disconnect();
        }

        private void Log(ReceivedManagementMessage message)
        {
            if (!message.IsByteCount)
                _logger.Info($"Management -> {message}");
        }

        private void Log(ManagementMessage message)
        {
            _logger.Info($"Management <- {message.LogText}");
        }
    }
}
