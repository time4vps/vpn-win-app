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
using System.Threading.Tasks;

namespace T4VPN.Common.Logging
{
    public class UnhandledExceptionLogging
    {
        private readonly ILogger _logger;

        public UnhandledExceptionLogging(ILogger logger)
        {
            _logger = logger;
        }

        public void CaptureUnhandledExceptions()
        {
            AppDomain.CurrentDomain.UnhandledException += LogUnhandledException;
        }

        public void CaptureTaskExceptions()
        {
            TaskScheduler.UnobservedTaskException += LogTaskException;
        }

        private void LogTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            _logger.Error($"Unobserved exception occured: {e.Exception.Message}");
            LogAggregatedException(e.Exception);
        }

        private void LogUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is AggregateException aggregateException)
            {
                _logger.Fatal($"Aggregate exception occured: {aggregateException.Message}");
                LogAggregatedException(aggregateException);
            }
            else
            {
                _logger.Fatal(e.ExceptionObject.ToString());
            }
        }

        private void LogAggregatedException(AggregateException e)
        {
            foreach (var ex in e.Flatten().InnerExceptions)
            {
                _logger.Fatal(ex.ToString());
            }
        }
    }
}
