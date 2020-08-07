﻿/*
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
using System.ComponentModel;
using System.Windows;

namespace T4VPN.Core.Wpf
{
    public static class DataBindingExtensions
    {
        public static void AddValueChanged<T>(this T obj, DependencyProperty property, EventHandler handler)
            where T : DependencyObject
        {
            var desc = DependencyPropertyDescriptor.FromProperty(property, typeof(T));
            desc.AddValueChanged(obj, handler);
        }

        public static void RemoveValueChanged<T>(this T obj, DependencyProperty property, EventHandler handler)
            where T : DependencyObject
        {
            var desc = DependencyPropertyDescriptor.FromProperty(property, typeof(T));
            desc.RemoveValueChanged(obj, handler);
        }
    }
}
