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

using System.Collections.Generic;

namespace T4VPN.Core.Api.Handlers.TlsPinning
{
    public class CachingReportClient : IReportClient
    {
        private readonly List<string> _cache = new List<string>();

        private readonly IReportClient _origin;

        public CachingReportClient(IReportClient origin)
        {
            _origin = origin;
        }

        public void Send(ReportBody body)
        {
            var hash = body.Hash();
            if (_cache.Contains(hash))
            {
                // return;
            }

            _cache.Add(hash);
            _origin.Send(body);
        }
    }
}
