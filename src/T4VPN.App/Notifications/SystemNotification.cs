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

using Caliburn.Micro;
using T4VPN.Core;
using T4VPN.Core.Events;
using T4VPN.Core.Settings;

namespace T4VPN.Notifications
{
    internal class SystemNotification : ISystemNotification
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IAppSettings _appSettings;
        private readonly AppExitHandler _appExitHandler;

        public SystemNotification(IEventAggregator eventAggregator, IAppSettings appSettings, AppExitHandler appExitHandler)
        {
            _appExitHandler = appExitHandler;
            _appSettings = appSettings;
            _eventAggregator = eventAggregator;
        }

        public void Show(string message)
        {
            if (!_appSettings.ShowNotifications || _appExitHandler.PendingExit)
                return;

            _eventAggregator.PublishOnUIThread(new ShowNotificationMessage(message));
        }
    }
}
