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
using T4VPN.Common;
using T4VPN.Common.Threading;
using T4VPN.Common.Vpn;
using T4VPN.Vpn.Common;

namespace T4VPN.Vpn.Connection
{
    /// <summary>
    /// Queues <see cref="StateChanged"/> events into sequence with requests.
    /// A wrapper around <see cref="ISingleVpnConnection"/>.
    /// </summary>
    /// <remarks>
    /// Other wrappers ahead of <see cref="QueueingEventsWrapper"/> will receive <see cref="StateChanged"/>
    /// events queued into single queue with requests. The next event will arrive only after previous
    /// one has passed the wrapper sequence ahead of <see cref="QueueingEventsWrapper"/>.
    ///
    /// Requests and events should be processed fast without delays.
    /// </remarks>
    internal class QueueingEventsWrapper : ISingleVpnConnection
    {
        private readonly ISingleVpnConnection _origin;
        private readonly ITaskQueue _taskQueue;

        public QueueingEventsWrapper(
            ITaskQueue taskQueue,
            ISingleVpnConnection origin)
        {
            _taskQueue = taskQueue;
            _origin = origin;

            _origin.StateChanged += Origin_StateChanged;
        }

        public event EventHandler<EventArgs<VpnState>> StateChanged;

        public InOutBytes Total => _origin.Total;

        public void Connect(VpnEndpoint endpoint, VpnCredentials credentials, VpnConfig config)
        {
            _origin.Connect(endpoint, credentials, config);
        }

        public void Disconnect(VpnError error)
        {
            _origin.Disconnect(error);
        }

        private void Origin_StateChanged(object sender, EventArgs<VpnState> e)
        {
            _taskQueue.Enqueue(() => OnStateChanged(e.Data));
        }

        private void OnStateChanged(VpnState state)
        {
            StateChanged?.Invoke(this, new EventArgs<VpnState>(state));
        }
    }
}