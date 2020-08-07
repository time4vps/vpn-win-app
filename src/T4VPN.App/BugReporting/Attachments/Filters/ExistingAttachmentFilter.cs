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
    internal class ExistingAttachmentFilter: IEnumerable<Attachment>
    {
        private readonly IReadOnlyCollection<Attachment> _existing;
        private readonly IEnumerable<Attachment> _source;

        public ExistingAttachmentFilter(IReadOnlyCollection<Attachment> existing, IEnumerable<Attachment> source)
        {
            _existing = existing;
            _source = source;
        }

        public IEnumerator<Attachment> GetEnumerator() => _source.Where(Condition).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private bool Condition(Attachment item) => !_existing.Contains(item);
    }
}
