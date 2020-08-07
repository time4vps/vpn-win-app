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

namespace T4VPN.P2PDetection
{
    /// <summary>
    /// Calculates P2P detection timeout value from P2P detection interval.
    /// </summary>
    internal class P2PDetectionTimeout
    {
        private readonly TimeSpan _interval;

        public P2PDetectionTimeout(TimeSpan interval)
        {
            _interval = interval;
        }

        /// <summary>
        /// P2P detection timeout value.
        /// P2P detection interval should fit two detection actions.
        /// </summary>
        public TimeSpan Value => new TimeSpan(_interval.Ticks / 2);

        public static implicit operator TimeSpan(P2PDetectionTimeout timeout) => timeout.Value;
    }
}
