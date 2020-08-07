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

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace T4VPN.Common.Helpers
{
    /// <summary>
    /// Concatenates multiple sequences.
    /// </summary>
    public class ConcatenatedSequence<T> : IEnumerable<T>
    {
        private readonly IEnumerable<T>[] _sources;

        public ConcatenatedSequence(params IEnumerable<T>[] sources)
        {
            Ensure.NotEmpty(sources, nameof(sources));

            _sources = sources;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _sources.SelectMany(i => i).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
