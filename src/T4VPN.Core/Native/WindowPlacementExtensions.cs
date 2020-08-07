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
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using T4VPN.Core.Native.Structures;

namespace T4VPN.Core.Native
{
    public static class WindowPlacementExtensions
    {
        private const int SW_HIDE = 0;
        private const int SW_SHOWNORMAL = 1;
        private const int SW_SHOWMINIMIZED = 2;

        [DllImport("user32.dll")]
        private static extern bool SetWindowPlacement(IntPtr hWnd, [In] WindowPlacement lpwndpl);

        [DllImport("user32.dll")]
        private static extern bool GetWindowPlacement(IntPtr hWnd, [In, Out] WindowPlacement lpwndpl);

        public static WindowPlacement GetWindowPlacement(this Window window)
        {
            var result = new WindowPlacement();
            GetWindowPlacement(new WindowInteropHelper(window).Handle, result);
            return result;
        }

        public static bool SetWindowPlacement(this Window window, WindowPlacement placement, bool restoreFromMinimizedState, bool hide)
        {
            placement.Length = Marshal.SizeOf(typeof(WindowPlacement));
            placement.Flags = 0;

            if (restoreFromMinimizedState)
                placement.ShowCommand = placement.ShowCommand == SW_SHOWMINIMIZED ? SW_SHOWNORMAL : placement.ShowCommand;

            if (hide)
                placement.ShowCommand = SW_HIDE;

            return SetWindowPlacement(new WindowInteropHelper(window).Handle, placement);
        }
    }
}
