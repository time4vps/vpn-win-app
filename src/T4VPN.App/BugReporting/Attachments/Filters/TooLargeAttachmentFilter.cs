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

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace T4VPN.BugReporting.Attachments.Filters
{
    internal class TooLargeAttachmentFilter: IEnumerable<Attachment>
    {
        private readonly long _maxSize;
        private readonly IEnumerable<Attachment> _source;

        public TooLargeAttachmentFilter(long maxSize, IEnumerable<Attachment> source)
        {
            _maxSize = maxSize;
            _source = source;
        }

        public IEnumerator<Attachment> GetEnumerator() => _source.Select(Filtered).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private Attachment Filtered(Attachment item)
        {
            return item.HasError() ? item : WithError(item);
        }

        private Attachment WithError(Attachment item) =>
            item.Length > _maxSize ? item.WithError(AttachmentErrorType.FileTooLarge) : item;
    }
}
