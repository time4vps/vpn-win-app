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

using GalaSoft.MvvmLight.CommandWpf;
using System.Windows.Input;

namespace T4VPN.Modals.Welcome
{
    public abstract class BaseWelcomeModalViewModel : BaseModalViewModel
    {
        private readonly Onboarding.Onboarding _onboarding;

        protected BaseWelcomeModalViewModel(Onboarding.Onboarding onboarding)
        {
            _onboarding = onboarding;
            TakeATourCommand = new RelayCommand(TakeATourAction);
        }

        public ICommand TakeATourCommand { get; set; }

        public override void CloseAction()
        {
            TryClose();
        }

        private void TakeATourAction()
        {
            _onboarding.Start();
            TryClose();
        }
    }
}
