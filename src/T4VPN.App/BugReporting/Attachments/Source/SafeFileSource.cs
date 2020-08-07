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

using T4VPN.Common.Extensions;
using T4VPN.Common.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security;

namespace T4VPN.BugReporting.Attachments.Source
{
    internal class SafeFileSource : IEnumerable<string>
    {
        private readonly ILogger _logger;
        private readonly IEnumerable<string> _origin;

        public SafeFileSource(ILogger logger, IEnumerable<string> origin)
        {
            _logger = logger;
            _origin = origin;
        }

        public IEnumerator<string> GetEnumerator()
        {
            try
            {
                return _origin.GetEnumerator();
            }
            catch (Exception e) when (e.IsFileAccessException() || e is SecurityException)
            {
                _logger.Warn("Failed to add attachment(s): " + e.CombinedMessage());
            }

            return Enumerable.Empty<string>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
