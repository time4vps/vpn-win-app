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
using T4VPN.Common;
using T4VPN.Common.Threading;
using T4VPN.Common.Vpn;
using T4VPN.Vpn.Common;

namespace T4VPN.Vpn.Connection
{
    /// <summary>
    /// Queues <see cref="Connect"/>, <see cref="Disconnect"/>, and <see cref="UpdateServers"/>
    /// requests into sequence with events.
    /// A wrapper around <see cref="ISingleVpnConnection"/>.
    /// </summary>
    /// <remarks>
    /// Other wrappers behind <see cref="QueuingRequestsWrapper"/> will receive <see cref="Connect"/>,
    /// <see cref="Disconnect"/>, and <see cref="UpdateServers"/> requests queued into single queue.
    /// The next request will arrive only after previous one has passed the wrapper sequence behind
    /// <see cref="QueuingRequestsWrapper"/>.
    ///
    /// Requests and events should be processed fast without delays.
    /// </remarks>
    public class QueuingRequestsWrapper : IVpnConnection
    {
        private readonly IVpnConnection _origin;
        private readonly ITaskQueue _taskQueue;

        public QueuingRequestsWrapper(
            ITaskQueue taskQueue,
            IVpnConnection origin)
        {
            _taskQueue = taskQueue;
            _origin = origin;
        }

        public event EventHandler<EventArgs<VpnState>> StateChanged
        {
            add => _origin.StateChanged += value;
            remove => _origin.StateChanged -= value;
        }

        public InOutBytes Total => _origin.Total;

        public void Connect(IReadOnlyList<VpnHost> servers, VpnConfig config, VpnProtocol protocol, VpnCredentials credentials)
        {
            _taskQueue.Enqueue(() => _origin.Connect(servers, config, protocol, credentials));
        }

        public void Disconnect(VpnError error = VpnError.None)
        {
            _taskQueue.Enqueue(() => _origin.Disconnect(error));
        }

        public void UpdateServers(IReadOnlyList<VpnHost> servers, VpnConfig config)
        {
            _taskQueue.Enqueue(() => _origin.UpdateServers(servers, config));
        }
    }
}