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

namespace T4VPN.Core.Wpf
{
    public class ControlExtension
    {
        public static bool GetFadeable(DependencyObject obj)
        {
            return (bool)obj.GetValue(FadeableProperty);
        }

        public static void SetFadeable(DependencyObject obj, bool value)
        {
            obj.SetValue(FadeableProperty, value);
        }

        public static readonly DependencyProperty FadeableProperty = DependencyProperty.RegisterAttached(
            "Fadeable",
            typeof(bool),
            typeof(ControlExtension),
            new UIPropertyMetadata(false));

        public static bool GetSpin(DependencyObject obj)
        {
            return (bool)obj.GetValue(SpinProperty);
        }

        public static void SetSpin(DependencyObject obj, bool value)
        {
            obj.SetValue(SpinProperty, value);
        }

        public static readonly DependencyProperty SpinProperty = DependencyProperty.RegisterAttached(
            "Spin",
            typeof(bool),
            typeof(ControlExtension),
            new UIPropertyMetadata(false));
    }
}
