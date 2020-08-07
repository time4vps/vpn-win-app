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
using T4VPN.Config.Url;
using T4VPN.Core.MVVM;
using T4VPN.Core.User;
using T4VPN.Trial;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace T4VPN.Sidebar.Trial
{
    public class TrialViewModel : ViewModel, ITrialStateAware, ITrialDurationAware
    {
        private TimeSpan _timeLeft;
        private PlanStatus _trialStatus;
        private readonly IActiveUrls _urls;

        public TrialViewModel(IActiveUrls urls)
        {
            _urls = urls;
            UpgradeCommand = new RelayCommand(UpgradeAction);
        }

        public PlanStatus TrialStatus
        {
            get => _trialStatus;
            set => Set(ref _trialStatus, value);
        }

        public TimeSpan TimeLeft
        {
            get => _timeLeft;
            set
            {
                _timeLeft = value;
                Set(ref _timeLeft, value);
            }
        }

        public ICommand UpgradeCommand { get; set; }

        private void UpgradeAction()
        {
            _urls.AccountUrl.Open();
        }

        public Task OnTrialStateChangedAsync(PlanStatus status)
        {
            TrialStatus = status;
            return Task.CompletedTask;
        }

        public void OnTrialSecondElapsed(TrialTickEventArgs e)
        {
            TimeLeft = DateTime.Now.AddSeconds(e.SecondsLeft) - DateTime.Now;
            OnPropertyChanged(nameof(TimeLeft));
        }
    }
}
