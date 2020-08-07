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
using T4VPN.Common.Extensions;

namespace T4VPN.Common.Logging
{
    public static class LoggerExtensions
    {
        public static void Logged(this ILogger logger, Action action, string message)
        {
            Logged(logger, action, () => message);
        }

        public static void Logged(this ILogger logger, Action action, Func<string> message)
        {
            bool Function()
            {
                action();
                return false;
            }

            Logged(logger, Function, message);
        }

        public static T Logged<T>(this ILogger logger, Func<T> function, string message)
        {
            return Logged(logger, function, () => message);
        }

        public static T Logged<T>(this ILogger logger, Func<T> function, Func<string> message)
        {
            try
            {
                return function();
            }
            catch (Exception ex)
            {
                logger.Error($"{message()}: {ex.CombinedMessage()}");
                throw;
            }
        }
    }
}
