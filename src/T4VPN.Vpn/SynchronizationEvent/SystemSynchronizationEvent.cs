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

using T4VPN.Common.Helpers;
using System.Threading;

namespace T4VPN.Vpn.SynchronizationEvent
{
    /// <summary>
    /// Synchronization event used to force exit of OpenVPN process.
    /// Wraps <see cref="EventWaitHandle"/>.
    /// </summary>
    internal class SystemSynchronizationEvent : ISynchronizationEvent
    {
        private EventWaitHandle _eventWaitHandle;

        public SystemSynchronizationEvent(EventWaitHandle eventWaitHandle)
        {
            Ensure.NotNull(eventWaitHandle, nameof(eventWaitHandle));

            _eventWaitHandle = eventWaitHandle;
        }

        public void Set()
        {
            _eventWaitHandle.Set();
        }

        public void Reset()
        {
            _eventWaitHandle.Reset();
        }

        public void Dispose()
        {
            _eventWaitHandle?.Dispose();
            _eventWaitHandle = null;
        }
    }
}
