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

using T4VPN.Core;
using System;
using System.Windows.Forms;
using System.Windows.Input;

namespace T4VPN.QuickLaunch
{
    internal partial class QuickLaunchWindow
    {
        public QuickLaunchWindow()
        {
            InitializeComponent();

            Deactivated += QuickLaunch_Deactivated;
            Activated += QuickLaunch_Activated;
        }

        private void QuickLaunch_Activated(object sender, EventArgs e)
        {
            var mouseX = Control.MousePosition.X * 96 / SystemParams.GetDpiX();
            var mouseY = Control.MousePosition.Y * 96 / SystemParams.GetDpiY();

            Left = mouseX - ActualWidth < 0 ? mouseX : mouseX - ActualWidth;
            Top = mouseY - ActualHeight < 0 ? mouseY : mouseY - ActualHeight;
        }

        private void QuickLaunch_Deactivated(object sender, EventArgs e)
        {
            Hide();
        }

        private void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Hide();
        }
    }
}
