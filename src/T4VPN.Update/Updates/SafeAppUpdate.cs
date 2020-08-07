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

using T4VPN.Common.Logging;
using System;
using System.Threading.Tasks;
using T4VPN.Common.Extensions;

namespace T4VPN.Update.Updates
{
    /// <summary>
    /// Suppresses and logs expected exceptions of <see cref="NotifyingAppUpdate.StartUpdating"/> method.
    /// </summary>
    public class SafeAppUpdate : INotifyingAppUpdate
    {
        private readonly INotifyingAppUpdate _origin;
        private readonly ILogger _logger;

        public SafeAppUpdate(ILogger logger, INotifyingAppUpdate origin)
        {
            _logger = logger;
            _origin = origin;
        }

        public event EventHandler<IAppUpdateState> StateChanged
        {
            add => _origin.StateChanged += value;
            remove => _origin.StateChanged -= value;
        }

        public void StartCheckingForUpdate(bool earlyAccess) => _origin.StartCheckingForUpdate(earlyAccess);

        public async Task StartUpdating(bool auto)
        {
            try
            {
                await _origin.StartUpdating(auto);
            }
            catch (AppUpdateException e)
            {
                _logger.Error(e.CombinedMessage());
            }
        }
    }
}
