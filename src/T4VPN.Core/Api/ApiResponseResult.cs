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

using T4VPN.Common.Abstract;
using System.Net;

namespace T4VPN.Core.Api
{
    public class ApiResponseResult<T> : Result<T>
    {
        protected ApiResponseResult(HttpStatusCode code, T value, bool success, string error) : base(value, success, error)
        {
            StatusCode = code;
        }

        public HttpStatusCode StatusCode { get; }

        public static ApiResponseResult<T> Ok(T value)
        {
            return new ApiResponseResult<T>(0, value, true, "");
        }

        public static ApiResponseResult<T> Fail(HttpStatusCode code, string error)
        {
            return new ApiResponseResult<T>(code, default(T), false, error);
        }

        public static ApiResponseResult<T> Fail(T value, HttpStatusCode code, string error)
        {
            return new ApiResponseResult<T>(code, value, false, error);
        }
    }
}
