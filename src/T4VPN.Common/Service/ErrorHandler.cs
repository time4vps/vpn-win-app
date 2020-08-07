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
using System.IO;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using T4VPN.Common.Extensions;
using T4VPN.Common.Logging;
using Sentry;
using Sentry.Protocol;

namespace T4VPN.Common.Service
{
    internal class ErrorHandler : IErrorHandler
    {
        private readonly ILogger _logger;

        public ErrorHandler(ILogger logger)
        {
            _logger = logger;
        }

        public bool HandleError(Exception e)
        {
            if (e.GetBaseException() is PipeException)
            {
                _logger.Warn(e.CombinedMessage());

                return false;
            }

            _logger.Error(e.CombinedMessage());

            SentrySdk.WithScope(scope =>
            {
                scope.SetTag("captured_in", "Service_ChannelDispatcher_ErrorHandler");
                scope.Level = SentryLevel.Warning;
                SentrySdk.CaptureException(e);
            });

            return false;
        }

        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
        }
    }
}
