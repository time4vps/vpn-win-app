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

using GalaSoft.MvvmLight;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace T4VPN.Core.MVVM
{
    public abstract class ViewModel : ViewModelBase, IViewModel
    {
        public delegate void ViewModelLoadedHandler(object sender, EventArgs e);

        public event ViewModelLoadedHandler ViewModelLoaded;

        public virtual string ViewModelName { get; set; }

        public void OnPropertyChanged([CallerMemberName]string name = null)
        {
            PropertyChangedHandler?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public void OnPropertyChangedAsync(string name)
        {
            PropertyChangedHandler.BeginInvoke(this, new PropertyChangedEventArgs(name), null, null);
        }

        public void OnViewModelLoaded(object sender)
        {
            ViewModelLoaded?.Invoke(sender, new EventArgs());
        }
    }
}
