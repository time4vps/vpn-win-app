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

namespace T4VPN.Common.Abstract
{
    public class Result
    {
        protected Result(bool success, string error = null, Exception exception = null)
        {
            Success = success;
            Error = error ?? "";
            Exception = exception;
        }

        public bool Success { get; }

        public string Error { get; }

        public Exception Exception { get; }

        public bool Failure => !Success;

        public static Result Fail(string message = null)
            => new Result(false, message);

        public static Result Fail(Exception exception)
            => new Result(false, "", exception);

        public static Result<T> Fail<T>(string message = "")
            => new Result<T>(default(T), false, message);

        public static Result Ok()
            => new Result(true, "");

        public static Result<T> Ok<T>(T value)
            => new Result<T>(value, true, "");
    }
}
