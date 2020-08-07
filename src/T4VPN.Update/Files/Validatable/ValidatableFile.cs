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

using System.IO;
using System.Threading.Tasks;

namespace T4VPN.Update.Files.Validatable
{
    /// <summary>
    /// Validates checksum of file.
    /// </summary>
    internal class ValidatableFile : IValidatableFile
    {
        public async Task<bool> Valid(string filename, string checkSum)
        {
            return Exists(filename) && await CheckSumValid(filename, checkSum);
        }

        private static bool Exists(string filename)
        {
            return !string.IsNullOrEmpty(filename) && File.Exists(filename);
        }

        private static async Task<bool> CheckSumValid(string filename, string expectedCheckSum)
        {
            var checkSum = await new FileCheckSum(filename).Value();
            return checkSum == expectedCheckSum;
        }
    }
}
