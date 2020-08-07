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
using System.IO;

namespace T4VPN.Common.OS.Processes
{
    public interface IOsProcess : IDisposable
    {
        string Name { get; }

        StreamWriter StandardInput { get; }

        int ExitCode { get; }

        void Start();

        bool HasExited();

        void WaitForExit(TimeSpan duration);

        void Kill();

        event EventHandler<EventArgs<string>> OutputDataReceived;

        event EventHandler<EventArgs<string>> ErrorDataReceived;

        event EventHandler Exited;
    }
}
