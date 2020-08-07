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

using System;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using T4VPN.Common.Threading;

namespace T4VPN.Core.Api.Handlers.TlsPinning
{
    public class ReportClient : IReportClient
    {
        private readonly HttpClient _httpClient;
        private readonly ITaskQueue _taskQueue = new SerialTaskQueue();
        private readonly Uri _uri;

        public ReportClient(Uri uri)
        {
            _uri = uri;
            _httpClient = new HttpClient();
        }

        public void Send(ReportBody body)
        {
            var content = GetJsonContent(body);
            _taskQueue.Enqueue(() =>
            {
                try
                {                    
                    _httpClient.PostAsync(_uri, content);
                }
                catch (HttpRequestException)
                {
                    // ignore
                }
            });
        }

        private StringContent GetJsonContent(object data)
        {
            return new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
        }
    }
}
