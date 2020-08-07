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

using T4VPN.Core.MVVM;
using System.Windows;
using System.Windows.Controls;

namespace T4VPN.Settings.SplitTunneling
{
    public partial class SplitTunnelingView : UserControl
    {
        public SplitTunnelingView()
        {
            InitializeComponent();

            DataContextChanged += This_DataContextChanged;
        }

        private void Apps_BringItemIntoView(object sender, BringItemIntoViewEventArgs e)
        {
            BringItemIntoView(AppsItemsControl, e.Item);
        }

        private void Ips_BringItemIntoView(object sender, BringItemIntoViewEventArgs e)
        {
            BringItemIntoView(IpsItemsControl, e.Item);
        }

        private void BringItemIntoView(ItemsControl itemsControl, object item)
        {
            if (itemsControl.ItemContainerGenerator.ContainerFromItem(item) is FrameworkElement container)
                container.BringIntoView();
        }

        private void This_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is SplitTunnelingViewModel oldViewModel)
            {
                WeakEventManager<AppListViewModel, BringItemIntoViewEventArgs>.RemoveHandler(oldViewModel.Apps, nameof(AppListViewModel.BringItemIntoView), Apps_BringItemIntoView);
                WeakEventManager<IpListViewModel, BringItemIntoViewEventArgs>.RemoveHandler(oldViewModel.Ips, nameof(IpListViewModel.BringItemIntoView), Ips_BringItemIntoView);
            }

            if (e.NewValue is SplitTunnelingViewModel newViewModel)
            {
                WeakEventManager<AppListViewModel, BringItemIntoViewEventArgs>.AddHandler(newViewModel.Apps, nameof(AppListViewModel.BringItemIntoView), Apps_BringItemIntoView);
                WeakEventManager<IpListViewModel, BringItemIntoViewEventArgs>.AddHandler(newViewModel.Ips, nameof(IpListViewModel.BringItemIntoView), Ips_BringItemIntoView);
            }
        }
    }
}
