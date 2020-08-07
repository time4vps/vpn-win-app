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
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using T4VPN.Core.Modals;
using T4VPN.ViewModels;

namespace T4VPN.Modals
{
    public abstract class BaseModalViewModel : LanguageAwareViewModel, IModal
    {
        private WindowState _windowState;
        private bool _loading;
        protected bool Loaded;

        public bool StayOnTop { get; protected set; }

        protected BaseModalViewModel()
        {
            CloseCommand = new RelayCommand(CloseAction);
            MinimizeCommand = new RelayCommand(MinimizeAction);
        }

        public ICommand CloseCommand { get; set; }
        public ICommand MinimizeCommand { get; set; }

        public bool Loading
        {
            get => _loading;
            set => Set(ref _loading, value);
        }

        public bool HideWindowControls { get; set; }

        public WindowState WindowState
        {
            get => _windowState;
            set => Set(ref _windowState, value);
        }

        public virtual void CloseAction()
        {
            TryClose(false);
        }

        public virtual void BeforeOpenModal(dynamic options)
        {
        }

        private void MinimizeAction()
        {
            WindowState = WindowState.Minimized;
        }
    }
}
