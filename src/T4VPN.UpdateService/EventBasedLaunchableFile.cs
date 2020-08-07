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
using System.IO;
using T4VPN.Common.Configuration;
using T4VPN.Common.OS.Event;
using T4VPN.Update;
using T4VPN.Update.Files.Launchable;
using Sentry;
using Sentry.Protocol;

namespace T4VPN.UpdateService
{
    public class EventBasedLaunchableFile : ILaunchableFile
    {
        private readonly SystemEventLog _systemEventLog;
        private readonly Config _config;
        private const int LaunchFileEventId = 1;

        public EventBasedLaunchableFile(SystemEventLog systemEventLog, Config config)
        {
            _config = config;
            _systemEventLog = systemEventLog;
        }

        public void Launch(string filename, string arguments)
        {
            try
            {
                File.WriteAllText(_config.UpdateFilePath, $"{filename}\n{arguments}");

                _systemEventLog.Log("Update app", LaunchFileEventId);
            }
            catch (Exception e)
            {
                SentrySdk.WithScope(scope =>
                {
                    scope.Level = SentryLevel.Error;
                    scope.SetTag("captured_in", "UpdateService_EventBasedLaunchableFile_Launch");
                    SentrySdk.CaptureException(e);
                });

                throw new AppUpdateException("Failed to start an update", e);
            }
        }
    }
}
