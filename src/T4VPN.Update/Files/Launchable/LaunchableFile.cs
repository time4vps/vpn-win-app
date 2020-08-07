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

using T4VPN.Common.OS.Processes;

namespace T4VPN.Update.Files.Launchable
{
    /// <summary>
    /// Starts new process requesting elevation.
    /// </summary>
    internal class LaunchableFile : ILaunchableFile
    {
        private readonly IOsProcesses _processes;

        public LaunchableFile(IOsProcesses processes)
        {
            _processes = processes;
        }

        public void Launch(string filename, string arguments)
        {
            _processes.ElevatedProcess(filename, arguments)
                .Start();
        }
    }
}
