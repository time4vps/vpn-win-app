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
using System.Threading.Tasks;

namespace T4VPN.Common.Threading
{
    public static class TaskCompletionSourceExtensions
    {
        public static Task Wrap(this TaskCompletionSource<object> source, Func<Task> action)
        {
            return Wrap(source, async () =>
            {
                await action();
                return null;
            });
        }

        public static async Task Wrap<T>(this TaskCompletionSource<T> source, Func<Task<T>> function)
        {
            try
            {
                source.SetResult(await function());
            }
            catch (OperationCanceledException)
            {
                source.SetCanceled();
            }
            catch (Exception e)
            {
                source.SetException(e);
            }
        }
    }
}
