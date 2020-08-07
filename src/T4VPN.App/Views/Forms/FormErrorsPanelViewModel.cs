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

using GalaSoft.MvvmLight.Command;
using T4VPN.Core.MVVM;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace T4VPN.Views.Forms
{
    public class FormErrorsPanelViewModel : ViewModel
    {
        public FormErrorsPanelViewModel()
        {
            CloseCommand = new RelayCommand(Close);
        }

        public ICommand CloseCommand { get; set; }

        private List<string> _errors;
        public List<string> Errors
        {
            get => _errors;
            set
            {
                Set(ref _errors, value);
                if (!Any())
                    Visible = false;
            }
        }

        public bool Any()
        {
            return Errors?.Any() == true;
        }

        private bool _visible;
        public bool Visible
        {
            get => _visible;
            private set => Set(ref _visible, value);
        }

        public void Show()
        {
            Visible = Any();
        }

        public void Close()
        {
            Visible = false;
        }
    }
}
