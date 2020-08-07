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
using T4VPN.Core.Servers.Name;

namespace T4VPN.Views.Controls
{
    public partial class ServerName
    {
        public static readonly DependencyProperty ConnectionNameProperty = DependencyProperty.Register(
            "ConnectionName",
            typeof(IName),
            typeof(ServerName),
            new PropertyMetadata(null));

        public static readonly DependencyProperty WrapElementsProperty = DependencyProperty.Register(
            "WrapElements",
            typeof(bool),
            typeof(ServerName),
            new PropertyMetadata(false));

        public static readonly DependencyProperty AlignCenterProperty = DependencyProperty.Register(
            "AlignCenter",
            typeof(bool),
            typeof(ServerName),
            new PropertyMetadata(false));

        public IName ConnectionName
        {
            get => (IName)GetValue(ConnectionNameProperty);
            set => SetValue(ConnectionNameProperty, value);
        }

        public bool WrapElements
        {
            get => (bool)GetValue(WrapElementsProperty);
            set => SetValue(WrapElementsProperty, value);
        }

        public bool AlignCenter
        {
            get => (bool)GetValue(AlignCenterProperty);
            set => SetValue(AlignCenterProperty, value);
        }

        public ServerName()
        {
            InitializeComponent();
        }
    }
}
