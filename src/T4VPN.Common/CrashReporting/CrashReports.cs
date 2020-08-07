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

using DeviceId;
using T4VPN.Common.Configuration;
using T4VPN.Common.Logging;
using T4VPN.Common.Service;
using Sentry;

namespace T4VPN.Common.CrashReporting
{
    public static class CrashReports
    {
        public static void Init(Config config, ILogger logger = null)
        {
            var options = new SentryOptions
            {
                Release = $"vpn.windows-{config.AppVersion}",
                AttachStacktrace = true,
                Dsn = !string.IsNullOrEmpty(GlobalConfig.SentryDsn) ? new Dsn(GlobalConfig.SentryDsn) : null,
            };

            if (logger != null)
            {
                options.Debug = true;
                options.DiagnosticLogger = new SentryDiagnosticLogger(logger);
            }

            options.BeforeSend = e =>
            {
                e.User.Id = new DeviceIdBuilder()
                    .AddProcessorId()
                    .AddMotherboardSerialNumber()
                    .ToString();

                return e;
            };

            SentrySdk.Init(options);
        }
    }
}
