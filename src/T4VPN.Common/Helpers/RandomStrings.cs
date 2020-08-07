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

namespace T4VPN.Common.Helpers
{
    /// <summary>
    /// Generates random alphanumeric strings.
    /// </summary>
    public class RandomStrings
    {
        private readonly Random _random = new Random();

        public string RandomString(int length)
        {
            Ensure.IsTrue(length >= 0);

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var randomChars = new char[length];

            for (var i = 0; i < randomChars.Length; i++)
            {
                randomChars[i] = chars[_random.Next(chars.Length)];
            }

            return new string(randomChars);
        }
    }
}
