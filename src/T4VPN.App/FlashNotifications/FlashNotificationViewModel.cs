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
using GalaSoft.MvvmLight.Command;
using T4VPN.Core.Auth;
using T4VPN.Core.MVVM;
using T4VPN.Notifications;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace T4VPN.FlashNotifications
{
    public class FlashNotificationViewModel :
        ViewModel,
        IHandle<ShowFlashMessage>,
        IHandle<HideFlashMessage>,
        ILogoutAware
    {
        private readonly ISystemNotification _systemNotification;

        public FlashNotificationViewModel(IEventAggregator eventAggregator, ISystemNotification systemNotification)
        {
            _systemNotification = systemNotification;
            eventAggregator.Subscribe(this);
            CloseMessageCommand = new RelayCommand<INotification>(CloseMessage);
            Notifications = new ObservableCollection<INotification>();
        }

        public ICommand CloseMessageCommand { get; set; }
        public ObservableCollection<INotification> Notifications { get; set; }

        public void Handle(ShowFlashMessage message)
        {
            ShowMessage(message.Message);
        }

        public void Handle(HideFlashMessage message)
        {
            CloseMessage(message.Message);
        }

        public void OnUserLoggedOut()
        {
            Notifications.Clear();
        }

        private void CloseMessage(INotification message)
        {
            var index = Notifications.IndexOf(message);
            if (index != -1)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Notifications.RemoveAt(index);
                });
            }
        }

        private void ShowMessage(INotification notification)
        {
            var index = Notifications.IndexOf(notification);
            if (index == -1)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Notifications.Add(notification);
                });
            }
            else
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Notifications.RemoveAt(index);
                    Notifications.Add(notification);
                });
            }

            _systemNotification.Show(notification.Message);
        }
    }
}
