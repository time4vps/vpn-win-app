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


using System.Diagnostics;

namespace T4VPN.Common.OS.Event
{
    public class SystemEventLog
    {
        private const string Source = "T4VPN";

        public void Log(string message, int eventId)
        {
            EnsureEventSourceExists();

            var log = new EventLog {Source = Source};

            log.WriteEntry(message, EventLogEntryType.Information, eventId);
        }

        private void EnsureEventSourceExists()
        {
            if (!EventLog.SourceExists(Source))
            {
                EventLog.CreateEventSource(Source, "Application");
            }
        }
    }
}
