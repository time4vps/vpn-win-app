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
using T4VPN.Common.Logging;
using Sentry.Extensibility;
using Sentry.Protocol;

namespace T4VPN.Common.Service
{
    public class SentryDiagnosticLogger : IDiagnosticLogger
    {
        private readonly ILogger _logger;

        public SentryDiagnosticLogger(ILogger logger)
        {
            _logger = logger;
        }

        public bool IsEnabled(SentryLevel level)
        {
            return level != SentryLevel.Debug;
        }

        public void Log(SentryLevel logLevel, string message, Exception exception = null, params object[] args)
        {
            var logMessage = $"SENTRY: {string.Format(message, args)}";

            switch (logLevel)
            {
                case SentryLevel.Debug:
                    _logger.Debug(logMessage);
                    break;
                case SentryLevel.Info:
                    _logger.Info(logMessage);
                    break;
                case SentryLevel.Warning:
                    _logger.Warn(logMessage);
                    break;
                case SentryLevel.Error:
                    _logger.Error(logMessage);
                    break;
                case SentryLevel.Fatal:
                    _logger.Fatal(logMessage);
                    break;
            }
        }
    }
}
