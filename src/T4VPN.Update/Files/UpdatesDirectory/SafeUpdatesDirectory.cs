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
using T4VPN.Common.Extensions;

namespace T4VPN.Update.Files.UpdatesDirectory
{
    /// <summary>
    /// Wraps expected exceptions of <see cref="UpdatesDirectory"/> into <see cref="AppUpdateException"/>.
    /// </summary>
    internal class SafeUpdatesDirectory : IUpdatesDirectory
    {
        private readonly IUpdatesDirectory _origin;

        public SafeUpdatesDirectory(IUpdatesDirectory origin)
        {
            _origin = origin;
        }

        public string Path
        {
            get
            {
                try
                {
                    return _origin.Path;
                }
                catch (Exception e) when (e.IsFileAccessException())
                {
                    throw new AppUpdateException("Failed to create updates directory", e);
                }
            }
        }

        public void Cleanup()
        {
            try
            {
                _origin.Cleanup();
            }
            catch (Exception e) when (e.IsFileAccessException())
            {
                throw new AppUpdateException("Failed to cleanup updates directory", e);
            }
        }
    }
}
