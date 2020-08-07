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

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using T4VPN.Common.Vpn;
using T4VPN.Config;
using T4VPN.Config.Url;
using T4VPN.Core;
using T4VPN.Core.Modals;
using T4VPN.Core.Models;
using T4VPN.Core.Profiles;
using T4VPN.Core.Service.Vpn;
using T4VPN.Core.Settings;
using T4VPN.Core.User;
using T4VPN.Core.Vpn;
using T4VPN.Modals;
using T4VPN.Profiles;
using T4VPN.Resources;
using T4VPN.Settings.ReconnectNotification;
using T4VPN.Settings.SplitTunneling;

namespace T4VPN.Settings
{
    public class SettingsModalViewModel : BaseModalViewModel, IVpnStateAware, IUserDataAware
    {
        private readonly IAppSettings _appSettings;
        private readonly IVpnManager _vpnManager;
        private readonly ProfileViewModelFactory _profileViewModelFactory;
        private readonly IDialogs _dialogs;
        private readonly IUserStorage _userStorage;
        private readonly IActiveUrls _urls;
        private readonly ILanguageProvider _languageProvider;
        private readonly IVpnConfig _vpnConfig;
        private readonly ReconnectState _reconnectState;

        private IReadOnlyList<ProfileViewModel> _autoConnectProfiles;
        private IReadOnlyList<ProfileViewModel> _quickConnectProfiles;
        private VpnStatus _vpnStatus;

        private ProfileViewModel _profileDisabledOption => new ProfileViewModel(new Profile
        {
            Id = "",
            Name = StringResources.Get("Settings_val_Disabled"),
            ColorCode = "#777783"
        });

        public SettingsModalViewModel(
            IAppSettings appSettings,
            IVpnManager vpnManager,
            IUserStorage userStorage,
            IDialogs dialogs,
            IActiveUrls urls,
            ILanguageProvider languageProvider,
            IVpnConfig vpnConfig,
            ReconnectState reconnectState,
            ProfileViewModelFactory profileViewModelFactory,
            SplitTunnelingViewModel splitTunnelingViewModel,
            CustomDnsListViewModel customDnsListViewModel)
        {
            _dialogs = dialogs;
            _appSettings = appSettings;
            _vpnManager = vpnManager;
            _profileViewModelFactory = profileViewModelFactory;
            _userStorage = userStorage;
            _urls = urls;
            _languageProvider = languageProvider;
            _vpnConfig = vpnConfig;
            _reconnectState = reconnectState;

            SplitTunnelingViewModel = splitTunnelingViewModel;
            Ips = customDnsListViewModel;

            ReconnectCommand = new RelayCommand(ReconnectAction);
            UpgradeCommand = new RelayCommand(UpgradeAction);
        }

        public ICommand ReconnectCommand { get; set; }
        public ICommand UpgradeCommand { get; set; }

        public IpListViewModel Ips { get; }

        public bool NetShieldVisible => _vpnConfig.NetShieldEnabled;

        private bool _changesPending;
        public bool ChangesPending
        {
            get => _changesPending;
            private set => Set(ref _changesPending, value);
        }

        private bool _disconnected;
        public bool Disconnected
        {
            get => _disconnected;
            private set
            {
                Set(ref _disconnected, value);
                SplitTunnelingViewModel.Disconnected = value;
            }
        }

        public int SelectedTabIndex
        {
            get => _appSettings.SettingsSelectedTabIndex;
            set => _appSettings.SettingsSelectedTabIndex = value;
        }

        private ProfileViewModel _autoConnect;
        public ProfileViewModel AutoConnect
        {
            get => _autoConnect;
            set
            {
                if (value == null)
                    return;

                Set(ref _autoConnect, value);
                _appSettings.AutoConnect = value.Id;
            }
        }

        private ProfileViewModel _quickConnect;
        public ProfileViewModel QuickConnect
        {
            get => _quickConnect;
            set
            {
                if (value == null)
                    return;

                Set(ref _quickConnect, value);
                _appSettings.QuickConnect = value.Id;
            }
        }

        public bool KillSwitch
        {
            get => _appSettings.KillSwitch;
            set
            {
                _appSettings.KillSwitch = value;
                NotifyOfPropertyChange();
            }
        }

        public bool Ipv6LeakProtection
        {
            get => _appSettings.Ipv6LeakProtection;
            set
            {
                _appSettings.Ipv6LeakProtection = value;
                NotifyOfPropertyChange();
            }
        }

        private bool _netShieldFullyEnabled;
        public bool NetShieldFullyEnabled
        {
            get => _netShieldFullyEnabled;
            set => Set(ref _netShieldFullyEnabled, value);
        }

        public bool DoHEnabled
        {
            get => _appSettings.DoHEnabled;
            set
            {
                _appSettings.DoHEnabled = value;
                NotifyOfPropertyChange();
            }
        }

        public bool CustomDnsEnabled
        {
            get => _appSettings.CustomDnsEnabled;
            set
            {
                if (value && _appSettings.NetShieldEnabled)
                {
                    var result = _dialogs.ShowQuestion(StringResources.Get("Settings_Connection_Warning_CustomDnsServer"));
                    if (result.HasValue && !result.Value)
                    {
                        return;
                    }

                    NetShieldEnabled = false;
                }

                _appSettings.CustomDnsEnabled = value;
                NotifyOfPropertyChange();
            }
        }

        public bool NetShieldEnabled
        {
            get => _appSettings.NetShieldEnabled;
            set
            {
                if (value && !_appSettings.NetShieldModalShown)
                {
                    _dialogs.ShowWarning(
                        StringResources.Get("NetShieldModal_msg"),
                        StringResources.Get("NetShieldModal_btn_GotIt"));

                    _appSettings.NetShieldModalShown = true;
                }

                if (value && _appSettings.CustomDnsEnabled)
                {
                    var result = _dialogs.ShowQuestion(StringResources.Get("Settings_Connection_Warning_NetShield"));
                    if (result.HasValue && !result.Value)
                    {
                        return;
                    }

                    CustomDnsEnabled = false;
                }

                _appSettings.NetShieldEnabled = value;
                NotifyOfPropertyChange();
            }
        }

        public int NetShieldMode
        {
            get => _appSettings.NetShieldMode;
            set
            {
                _appSettings.NetShieldMode = value;
                NotifyOfPropertyChange();
            }
        }

        public List<KeyValuePair<StartMinimizedMode, string>> StartMinimizedModes => new List<KeyValuePair<StartMinimizedMode, string>>
        {
            new KeyValuePair<StartMinimizedMode, string>(StartMinimizedMode.Disabled, StringResources.Get("StartMinimizedMode_val_Disabled")),
            new KeyValuePair<StartMinimizedMode, string>(StartMinimizedMode.ToSystray, StringResources.Get("StartMinimizedMode_val_ToSystray")),
            new KeyValuePair<StartMinimizedMode, string>(StartMinimizedMode.ToTaskbar, StringResources.Get("StartMinimizedMode_val_ToTaskbar")),
        };

        public StartMinimizedMode StartMinimized
        {
            get => _appSettings.StartMinimized;
            set => _appSettings.StartMinimized = value;
        }

        public bool EarlyAccess
        {
            get => _appSettings.EarlyAccess;
            set => _appSettings.EarlyAccess = value;
        }

        public bool ShowNotifications
        {
            get => _appSettings.ShowNotifications;
            set => _appSettings.ShowNotifications = value;
        }

        public bool StartOnStartup
        {
            get => _appSettings.StartOnStartup;
            set => _appSettings.StartOnStartup = value;
        }

        public string SelectedProtocol
        {
            get => _appSettings.OvpnProtocol;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    return;
                }

                _appSettings.OvpnProtocol = value;
            }
        }

        public List<KeyValuePair<string, string>> Languages
        {
            get
            {
                var langs = new List<KeyValuePair<string, string>>();
                foreach (var lang in _languageProvider.GetAll())
                {
                    langs.Add(new KeyValuePair<string, string>(lang, StringResources.Get($"Language_{lang}")));
                }

                return GetSorted(langs);
            }
        }

        public string SelectedLanguage
        {
            get => _appSettings.Language;
            set
            {
                if (value == null || _appSettings.Language == value)
                {
                    return;
                }

                _appSettings.Language = value;
            }
        }

        public List<KeyValuePair<string, string>> Protocols => new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("auto",
                StringResources.Get("Settings_Connection_DefaultProtocol_val_Auto")),
            new KeyValuePair<string, string>("tcp",
                StringResources.Get("Settings_Connection_DefaultProtocol_val_Tcp")),
            new KeyValuePair<string, string>("udp",
                StringResources.Get("Settings_Connection_DefaultProtocol_val_Udp")),
        };

        public IReadOnlyList<ProfileViewModel> AutoConnectProfiles
        {
            get => _autoConnectProfiles;
            set => Set(ref _autoConnectProfiles, value);
        }

        public IReadOnlyList<ProfileViewModel> QuickConnectProfiles
        {
            get => _quickConnectProfiles;
            set => Set(ref _quickConnectProfiles, value);
        }

        public SplitTunnelingViewModel SplitTunnelingViewModel { get; }

        protected override async void OnActivate()
        {
            SetDisconnected();
            SetKillSwitchEnabled();
            await LoadProfiles();
            SplitTunnelingViewModel.OnActivate();
            RefreshReconnectRequiredState(string.Empty);
        }

        public Task OnVpnStateChanged(VpnStateChangedEventArgs e)
        {
            _vpnStatus = e.State.Status;

            SetDisconnected();

            return Task.CompletedTask;
        }

        public void OpenAdvancedTab()
        {
            SelectedTabIndex = 2;
        }

        public override async void OnAppSettingsChanged(PropertyChangedEventArgs e)
        {
            base.OnAppSettingsChanged(e);

            if (e.PropertyName.Equals(nameof(IAppSettings.StartOnStartup)))
            {
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(StartOnStartup)));
            }
            else if (e.PropertyName.Equals(nameof(IAppSettings.Profiles)))
            {
                await LoadProfiles();
            }
            else if (e.PropertyName.Equals(nameof(IAppSettings.SplitTunnelMode)))
            {
                SetKillSwitchEnabled();
            }
            else if (e.PropertyName.Equals(nameof(IAppSettings.SplitTunnelingEnabled)))
            {
                SetKillSwitchEnabled();
            }
            else if (e.PropertyName.Equals(nameof(IAppSettings.Language)))
            {
                OnLanguageChanged();
            }

            RefreshReconnectRequiredState(e.PropertyName);
        }

        public async void OnLanguageChanged()
        {
            NotifyOfPropertyChange(() => Languages);

            NotifyOfPropertyChange(() => Protocols);
            NotifyOfPropertyChange(() => SelectedProtocol);

            NotifyOfPropertyChange(() => StartMinimizedModes);
            NotifyOfPropertyChange(() => StartMinimized);

            await LoadProfiles();
        }

        public void OnUserDataChanged()
        {
            SetNetShieldPermissions();
        }

        private void SetKillSwitchEnabled()
        {
            if (!_appSettings.SplitTunnelingEnabled)
                return;

            KillSwitch = false;
        }

        private void SetDisconnected()
        {
            Disconnected = _vpnStatus == VpnStatus.Disconnecting ||
                           _vpnStatus == VpnStatus.Disconnected;
        }

        private void RefreshReconnectRequiredState(string settingChanged)
        {
            ChangesPending = _reconnectState.Required(settingChanged);
        }

        private async Task LoadProfiles()
        {
            await LoadAutoConnectProfiles();
            await LoadQuickConnectProfiles();
            AutoConnect = GetSelectedAutoConnectProfile();
            QuickConnect = GetSelectedQuickConnectProfile();
        }

        private async Task LoadAutoConnectProfiles()
        {
            var profiles = new List<ProfileViewModel>
            {
                _profileDisabledOption
            };
            profiles.AddRange(await GetProfiles());

            AutoConnectProfiles = profiles;
        }

        private async Task LoadQuickConnectProfiles()
        {
            QuickConnectProfiles = await GetProfiles();
        }

        private async Task<List<ProfileViewModel>> GetProfiles()
        {
            return (await _profileViewModelFactory.GetProfiles())
                .OrderByDescending(p => p.IsPredefined)
                .ThenBy(p => p.Name)
                .ToList();
        }

        private ProfileViewModel GetSelectedAutoConnectProfile()
        {
            var profile = AutoConnectProfiles.FirstOrDefault(p => p.Id == _appSettings.AutoConnect);
            if (profile == null)
            {
                return _profileDisabledOption;
            }

            return profile;
        }

        private ProfileViewModel GetSelectedQuickConnectProfile()
        {
            var profile = QuickConnectProfiles.FirstOrDefault(p => p.Id == _appSettings.QuickConnect);
            if (profile != null)
            {
                return profile;
            }

            return QuickConnectProfiles.FirstOrDefault(p => p.IsPredefined && p.Id == "Fastest");
        }

        private async void ReconnectAction()
        {
            await _vpnManager.Reconnect();
        }

        private void SetNetShieldPermissions()
        {
            NetShieldFullyEnabled = _userStorage.User().MaxTier > 0;
            if (!NetShieldFullyEnabled)
            {
                NetShieldMode = 1;
            }
        }

        private void UpgradeAction()
        {
            _urls.AccountUrl.Open();
        }

        private List<KeyValuePair<string, string>> GetSorted(List<KeyValuePair<string, string>> collection)
        {
            var sorted = collection.OrderBy(l => l.Value);
            var list = new List<KeyValuePair<string, string>>();
            foreach (var item in sorted)
            {
                list.Add(item);
            }

            return list;
        }
    }
}
