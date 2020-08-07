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
using T4VPN.Common.Threading;
using T4VPN.Core.Service;
using T4VPN.UpdateServiceContract;
using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace T4VPN.Core.Update
{
    public class ServiceClient
    {
        private readonly ServiceChannelFactory _channelFactory;
        private readonly ILogger _logger;
        private ServiceChannel<IUpdateContract> _channel;
        private readonly UpdateEvents _updateEvents;

        public ServiceClient(ILogger logger, ServiceChannelFactory channelFactory, UpdateEvents updateEvents)
        {
            _channelFactory = channelFactory;
            _logger = logger;
            _updateEvents = updateEvents;
        }

        public event EventHandler<UpdateStateContract> UpdateStateChanged
        {
            add => _updateEvents.UpdateStateChanged += value;
            remove => _updateEvents.UpdateStateChanged -= value;
        }

        public Task CheckForUpdate(bool earlyAccess) => Invoke(p => p.CheckForUpdate(earlyAccess).Wrap());

        public Task StartUpdating(bool auto) => Invoke(p => p.Update(auto).Wrap());

        private async Task<T> Invoke<T>(Func<IUpdateContract, Task<T>> serviceCall)
        {
            var retryCount = 1;
            while (true)
            {
                try
                {
                    var channel = GetChannel();
                    return await serviceCall(channel.Proxy);
                }
                catch (Exception ex) when (IsCommunicationException(ex))
                {
                    CloseChannel();
                    if (retryCount <= 0)
                    {
                        throw;
                    }
                    _logger.Error("Retrying Invoke due to: " + ex.Message);
                }

                retryCount--;
            }
        }

        private ServiceChannel<IUpdateContract> GetChannel()
        {
            return _channel ?? (_channel = NewChannel());
        }

        private ServiceChannel<IUpdateContract> NewChannel()
        {
            var channel = _channelFactory.Create<IUpdateContract>(
                "t4vpn-update-service/update",
                _updateEvents);

            RegisterCallback(channel);

            return channel;
        }

        private void RegisterCallback(ServiceChannel<IUpdateContract> channel)
        {
            try
            {
                channel.Proxy.RegisterCallback();
            }
            catch (Exception)
            {
                channel.Dispose();
                throw;
            }
        }

        private void CloseChannel()
        {
            _channel?.Dispose();
            _channel = null;
        }

        private static bool IsCommunicationException(Exception ex) =>
            ex is CommunicationException ||
            ex is TimeoutException ||
            ex is ObjectDisposedException ode && ode.ObjectName == "System.ServiceModel.Channels.ClientFramingDuplexSessionChannel";
    }
}
