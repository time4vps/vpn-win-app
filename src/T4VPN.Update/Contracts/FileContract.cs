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

namespace T4VPN.Update.Contracts
{
    internal class FileContract : IEquatable<FileContract>
    {
        public string Url;

        public string Sha512CheckSum;

        public string Arguments;

        #region IEquatable

        public bool Equals(FileContract other)
        {
            if (other == null)
                return false;

            return string.Equals(Url, other.Url) &&
                   string.Equals(Sha512CheckSum, other.Sha512CheckSum) &&
                   string.Equals(Arguments, other.Arguments);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as FileContract);
        }

        public override int GetHashCode()
        {
            throw new InvalidOperationException();
        }

        #endregion
    }
}
