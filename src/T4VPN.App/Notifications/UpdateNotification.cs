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
using Caliburn.Micro;
using T4VPN.Core.Auth;
using T4VPN.Core.Update;
using T4VPN.FlashNotifications;
using T4VPN.Resources;

namespace T4VPN.Notifications
{
    public class UpdateNotification : IUpdateStateAware
    {
        private readonly ISystemNotification _systemNotification;
        private readonly TimeSpan _remindInterval;
        private readonly UserAuth _userAuth;
        private readonly IEventAggregator _eventAggregator;
        private readonly UpdateFlashNotificationViewModel _notificationViewModel;
        private DateTime _lastNotified = DateTime.MinValue;

        public UpdateNotification(
            TimeSpan remindInterval,
            ISystemNotification systemNotification,
            UserAuth userAuth,
            IEventAggregator eventAggregator,
            UpdateFlashNotificationViewModel notificationViewModel)
        {
            _systemNotification = systemNotification;
            _remindInterval = remindInterval;
            _userAuth = userAuth;
            _eventAggregator = eventAggregator;
            _notificationViewModel = notificationViewModel;
        }

        public void OnUpdateStateChanged(UpdateStateChangedEventArgs e)
        {
            if (e.Ready)
            {
                if (RemindRequired(e) && _userAuth.LoggedIn)
                {
                    Show();
                }
            }
            else
            {
                Hide();
            }
        }

        private bool RemindRequired(UpdateStateChangedEventArgs e)
        {
            return e.Status == UpdateStatus.Ready && (e.ManualCheck ||
                DateTime.Now - _lastNotified >= _remindInterval);
        }

        private void Show()
        {
            _lastNotified = DateTime.Now;
            _systemNotification.Show(StringResources.Get("Dialogs_Update_msg_NewAppVersion"));
            _eventAggregator.PublishOnUIThread(new ShowFlashMessage(_notificationViewModel));
        }

        private void Hide()
        {
            _eventAggregator.PublishOnUIThread(new HideFlashMessage(_notificationViewModel));
        }
    }
}
