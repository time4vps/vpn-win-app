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
using Microsoft.Win32;

namespace T4VPN.Common.OS.Registry
{
    public class CurrentUserStartupRecord : IStartupRecord
    {
        private const string RunKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";

        private readonly string _name;
        private readonly string _command;

        public CurrentUserStartupRecord(string name, string command)
        {
            _name = name;
            _command = command;
        }

        public bool Exists() => Execute(key => key?.GetValue(_name) != null);

        public bool Valid() => Execute(key => key?.GetValue(_name) is string command && _command == command);

        public void Create() => Execute(key => key?.SetValue(_name, _command));

        public void Remove()
        {
            Execute(key =>
            {
                if (key.GetValue(_name) == null)
                    return;

                key.DeleteValue(_name);
            });
        }

        private T Execute<T>(Func<RegistryKey, T> function)
        {
            using var registryKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(RunKey, false);
            return function(registryKey);
        }

        private void Execute(Action<RegistryKey> action)
        {
            using var registryKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(RunKey, true);
            action(registryKey);
        }
    }
}
