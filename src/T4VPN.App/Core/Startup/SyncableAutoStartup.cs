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

using T4VPN.Common.Threading;
using T4VPN.Core.Settings;

namespace T4VPN.Core.Startup
{
    internal class SyncableAutoStartup : ISyncableAutoStartup
    {
        private readonly IAppSettings _appSettings;
        private readonly IScheduler _scheduler;
        private readonly IAutoStartup _autoStartup;

        private bool _syncing;

        public SyncableAutoStartup(
            IAppSettings appSettings,
            IScheduler scheduler,
            IAutoStartup autoStartup)
        {
            _appSettings = appSettings;
            _scheduler = scheduler;
            _autoStartup = autoStartup;
        }

        public void Sync()
        {
            if (_syncing)
                return;

            SyncForward();

            if (SyncBackRequired())
                _scheduler.Schedule(SyncBack);
        }

        private void SyncForward()
        {
            _autoStartup.Enabled = _appSettings.StartOnStartup;
        }

        private bool SyncBackRequired()
        {
            return _autoStartup.Enabled != _appSettings.StartOnStartup;
        }

        private void SyncBack()
        {
            _syncing = true;

            _appSettings.StartOnStartup = _autoStartup.Enabled;

            _syncing = false;
        }
    }
}
