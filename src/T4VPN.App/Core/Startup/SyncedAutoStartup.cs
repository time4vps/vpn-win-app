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

using T4VPN.Core.Settings;
using System.ComponentModel;

namespace T4VPN.Core.Startup
{
    internal class SyncedAutoStartup : ISyncableAutoStartup, ISettingsAware
    {
        private readonly ISyncableAutoStartup _origin;

        public SyncedAutoStartup(ISyncableAutoStartup origin)
        {
            _origin = origin;
        }

        public void Sync()
        {
            _origin.Sync();
        }

        public void OnAppSettingsChanged(PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IAppSettings.StartOnStartup))
            {
                Sync();
            }
        }
    }
}
