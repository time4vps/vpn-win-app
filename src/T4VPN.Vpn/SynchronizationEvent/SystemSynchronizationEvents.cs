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
using System;
using System.IO;
using System.Security.AccessControl;
using System.Threading;

namespace T4VPN.Vpn.SynchronizationEvent
{
    /// <summary>
    /// Provides access to system synchronization events.
    /// </summary>
    internal class SystemSynchronizationEvents : ISynchronizationEvents
    {
        private readonly ILogger _logger;

        public SystemSynchronizationEvents(ILogger logger)
        {
            _logger = logger;
        }

        public ISynchronizationEvent SynchronizationEvent(string eventName)
        {
            try
            {
                if (EventWaitHandle.TryOpenExisting(eventName, EventWaitHandleRights.Modify, out var eventWaitHandle))
                    return new SystemSynchronizationEvent(eventWaitHandle);
            }
            catch (UnauthorizedAccessException ex)
            {
                LogException(ex);
            }
            catch (IOException ex)
            {
                LogException(ex);
            }
            return new NullSynchronizationEvent(); 

            void LogException(Exception ex)
            {
                _logger.Warn($"Synchronization: Failed to open event {eventName}. {ex.Message}");
            }
        }
    }
}
