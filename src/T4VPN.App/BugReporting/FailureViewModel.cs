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

using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using T4VPN.Core.Modals;
using T4VPN.Core.MVVM;
using T4VPN.Modals;

namespace T4VPN.BugReporting
{
    public class FailureViewModel : ViewModel
    {
        private readonly IModals _modals;

        public FailureViewModel(IModals modals)
        {
            _modals = modals;
            TroubleshootCommand = new RelayCommand(TroubleshootAction);
        }

        public ICommand TroubleshootCommand { get; set; }

        private void TroubleshootAction()
        {
            _modals.Show<TroubleshootModalViewModel>();
        }
    }
}
