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
using System.Collections.Generic;
using System.Configuration.Install;
using System.Linq;
using System.Reflection;

namespace T4VPN.Service.Start
{
    public class CommandLineStartStrategy
    {
        public void Start(IEnumerable<string> commandLineArgs)
        {
            var arg = commandLineArgs.FirstOrDefault();
            if (arg == null)
                return;

            if (arg.Equals("install", StringComparison.OrdinalIgnoreCase))
            {
                ManagedInstallerClass.InstallHelper(new[] { Assembly.GetExecutingAssembly().Location });
                return;
            }

            if (arg.Equals("uninstall", StringComparison.OrdinalIgnoreCase))
            {
                ManagedInstallerClass.InstallHelper(new[] { "/u", Assembly.GetExecutingAssembly().Location });
            }
        }
    }
}
