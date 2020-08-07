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
using T4VPN.Common.Helpers;

namespace T4VPN.Common.Extensions
{
    public static class TimeSpanExtensions
    {
        private static Random Random = new Random();

        public static TimeSpan RandomizedWithDeviation(this TimeSpan value, double deviation)
        {
            Ensure.IsTrue(value > TimeSpan.Zero, $"{nameof(value)} must be positive");
            Ensure.IsTrue(deviation >= 0 && deviation < 1, $"{nameof(deviation)} must be between zero and one");

            return value + TimeSpan.FromMilliseconds(value.TotalMilliseconds * deviation * (2.0 * Random.NextDouble() - 1.0));
        }
    }
}
