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

using T4VPN.Core.Modals;
using T4VPN.Resources;

namespace T4VPN.Modals.Dialogs
{
    internal class Dialogs : IDialogs
    {
        private readonly IModals _modals;
        private readonly QuestionModalViewModel _questionViewModel;
        private readonly WarningModalViewModel _warningViewModel;

        public Dialogs(IModals modals, WarningModalViewModel warningViewModel, QuestionModalViewModel questionViewModel)
        {
            _modals = modals;
            _warningViewModel = warningViewModel;
            _questionViewModel = questionViewModel;
        }

        public bool? ShowWarning(string message)
        {
            var settings = DialogSettings.FromMessage(message)
                .WithPrimaryButtonText(StringResources.Get("Dialogs_btn_Close"));

            _warningViewModel.ApplySettings(settings);
            return _modals.Show<WarningModalViewModel>();
        }

        public bool? ShowWarning(string message, string buttonLabel)
        {
            var settings = DialogSettings.FromMessage(message).WithPrimaryButtonText(buttonLabel);
            _warningViewModel.ApplySettings(settings);
            return _modals.Show<WarningModalViewModel>();
        }

        public bool? ShowQuestion(string message)
        {
            var settings = DialogSettings.FromMessage(message);
            _questionViewModel.ApplySettings(settings);
            return _modals.Show<QuestionModalViewModel>();
        }

        public bool? ShowQuestion(IDialogSettings settings)
        {
            _questionViewModel.ApplySettings(settings);
            return _modals.Show<QuestionModalViewModel>();
        }
    }
}
