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

using T4VPN.Common.Helpers;
using System;

namespace T4VPN.BugReporting.Attachments
{
    public class Attachment : IEquatable<Attachment>
    {
        public string Name { get; }

        public string Path { get; }

        public long Length { get; }

        public AttachmentErrorType ErrorType { get; }

        public Attachment(string filePath) : this(System.IO.Path.GetFileName(filePath), filePath, 0, AttachmentErrorType.None)
        { }

        private Attachment(string name, string path, long length, AttachmentErrorType errorType)
        {
            Ensure.NotEmpty(name, nameof(name));
            Ensure.NotEmpty(path, nameof(path));

            Name = name;
            Path = path;
            Length = length;
            ErrorType = errorType;
        }

        public Attachment WithLength(long length)
        {
            return new Attachment(Name, Path, length, ErrorType);
        }

        public Attachment WithError(AttachmentErrorType errorType)
        {
            return new Attachment(Name, Path, Length, errorType);
        }

        #region IEquatable

        public bool Equals(Attachment other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return string.Equals(Path, other.Path, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (!ReferenceEquals(GetType(), obj.GetType())) return false;

            return Equals((Attachment) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Start
                .Hash(Path.ToUpperInvariant());
        }

        #endregion
    }
}
