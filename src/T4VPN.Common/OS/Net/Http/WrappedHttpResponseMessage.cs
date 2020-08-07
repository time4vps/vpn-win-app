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

using System.Net.Http;

namespace T4VPN.Common.OS.Net.Http
{
    internal class WrappedHttpResponseMessage : IHttpResponseMessage
    {
        private readonly HttpResponseMessage _httpResponseMessage;

        public WrappedHttpResponseMessage(HttpResponseMessage httpResponseMessage)
        {
            _httpResponseMessage = httpResponseMessage;
        }

        private IHttpContent _content;
        public IHttpContent Content => _content ?? (_content = new WrappedHttpContent(_httpResponseMessage.Content));

        public bool IsSuccessStatusCode => _httpResponseMessage.IsSuccessStatusCode;

        public void Dispose()
        {
            _content = null;
            _httpResponseMessage.Dispose();
        }
    }
}
