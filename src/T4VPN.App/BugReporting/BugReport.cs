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

using T4VPN.BugReporting.Attachments;
using T4VPN.Common.Abstract;
using T4VPN.Common.Extensions;
using T4VPN.Core.Api;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace T4VPN.BugReporting
{
    public class BugReport
    {
        private readonly IApiClient _apiClient;
        private readonly IEnumerable<File> _attachments;

        public BugReport(IApiClient apiClient, Attachments.Attachments attachments)
        {
            _apiClient = apiClient;
            _attachments = new AttachmentsToApiFiles(attachments.Items);
        }

        public async Task<Result> Send(KeyValuePair<string, string>[] fields)
        {
            try
            {
                return await _apiClient.ReportBugAsync(fields, _attachments);
            }
            catch (Exception e) when (e is HttpRequestException || e.IsFileAccessException())
            {
                return Result.Fail(e.Message);
            }
        }
    }
}
