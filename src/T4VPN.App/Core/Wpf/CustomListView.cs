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

using System.Windows;
using System.Windows.Controls;

namespace T4VPN.Core.Wpf
{
    internal class CustomListView : ListView
    {
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            SetItemIndex(element, ItemContainerGenerator.IndexFromContainer(element));
        }

        public static int GetItemIndex(DependencyObject obj)
        {
            return (int)obj.GetValue(ItemIndexProperty);
        }

        protected static void SetItemIndex(DependencyObject obj, int value)
        {
            obj.SetValue(ItemIndexPropertyKey, value);
        }

        private static readonly DependencyPropertyKey ItemIndexPropertyKey =
            DependencyProperty.RegisterAttachedReadOnly("ItemIndex", typeof(int), typeof(CustomListView), new PropertyMetadata(-1));

        public static readonly DependencyProperty ItemIndexProperty = ItemIndexPropertyKey.DependencyProperty;
    }
}
