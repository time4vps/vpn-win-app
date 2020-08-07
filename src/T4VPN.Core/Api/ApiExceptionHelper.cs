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
using System.Net.Http;
using System.Net.Sockets;
using System.Security.Authentication;
using Newtonsoft.Json;
using Polly.Timeout;

namespace T4VPN.Core.Api
{
    public static class ApiExceptionHelper
    {
        public static bool IsApiCommunicationException(this Exception ex)
        {
            return ex is HttpRequestException ||
                   ex is JsonException ||
                   ex is OperationCanceledException ||
                   ex is TimeoutRejectedException ||
                   ex is SocketException;
        }

        public static bool IsPotentialBlocking(this Exception e)
        {
            return e is TimeoutException ||
                   e is TimeoutRejectedException ||
                   e.GetBaseException() is AuthenticationException;
        }
    }
}
