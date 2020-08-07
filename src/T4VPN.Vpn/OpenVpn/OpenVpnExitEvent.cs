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

using T4VPN.Common.Logging;
using T4VPN.Vpn.SynchronizationEvent;

namespace T4VPN.Vpn.OpenVpn
{
    /// <summary>
    /// Signals OpenVPN process to exit using synchronization event.
    /// </summary>
    internal class OpenVpnExitEvent
    {
        private readonly ILogger _logger;
        private readonly ISynchronizationEvents _synchronizationEvents;
        private readonly string _exitEventName;

        public OpenVpnExitEvent(ILogger logger, ISynchronizationEvents synchronizationEvents, string exitEventName)
        {
            _logger = logger;
            _synchronizationEvents = synchronizationEvents;
            _exitEventName = exitEventName;
        }

        public void Signal()
        {
            _logger.Info($"OpenVPN <- Signaling {_exitEventName}");
            using (var exitEvent = _synchronizationEvents.SynchronizationEvent(_exitEventName))
            {
                exitEvent.Set();
                exitEvent.Reset();
            }
        }
    }
}
