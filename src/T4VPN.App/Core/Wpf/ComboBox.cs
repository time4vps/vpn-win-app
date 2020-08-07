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
using ComboBox = System.Windows.Controls.ComboBox;

namespace T4VPN.Core.Wpf
{
    public class Combobox : ComboBox
    {
        public Thickness ActiveBorderThickness
        {
            get => (Thickness)GetValue(ActiveBorderThicknessProperty);
            set => SetValue(ActiveBorderThicknessProperty, value);
        }

        public static readonly DependencyProperty ActiveBorderThicknessProperty = DependencyProperty.Register(
            "ActiveBorderThickness",
            typeof(Thickness),
            typeof(Combobox),
            new PropertyMetadata(new Thickness(0,0,0,0)));

        public Thickness PopupPadding
        {
            get => (Thickness)GetValue(PopupPaddingProperty);
            set => SetValue(PopupPaddingProperty, value);
        }

        public static readonly DependencyProperty PopupPaddingProperty = DependencyProperty.Register(
            "PopupPadding",
            typeof(Thickness),
            typeof(Combobox),
            new PropertyMetadata(new Thickness(0, 0, 0, 0)));

        public string PopupPlacement
        {
            get => (string)GetValue(PopupPlacementProperty);
            set => SetValue(PopupPlacementProperty, value);
        }

        public static readonly DependencyProperty PopupPlacementProperty = DependencyProperty.Register(
            "PopupPlacement",
            typeof(string),
            typeof(Combobox),
            new PropertyMetadata("Bottom"));
    }
}
