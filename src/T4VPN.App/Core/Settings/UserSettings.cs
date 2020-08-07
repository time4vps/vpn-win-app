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

using T4VPN.Core.Abstract;
using T4VPN.Core.Storage;
using System.Collections.Generic;
using System.Linq;

namespace T4VPN.Core.Settings
{
    internal class UserSettings : ISettingsStorage, ISupportsMigration
    {
        private const string SettingsVersionKey = "SettingsVersion";
        private const string UserSettingsMigratedKey = "UserSettingsMigrated";

        private readonly Common.Configuration.Config _appConfig;
        private readonly List<IMigration> _migrations = new List<IMigration>();
        private readonly ISettingsStorage _storage;
        private readonly ISettingsStorage _appSettings;
        private bool _isMigrating;

        public UserSettings(Common.Configuration.Config appConfig, PerUserSettings perUserSettings, ISettingsStorage appSettings)
        {
            _appConfig = appConfig;
            _storage = perUserSettings;
            _appSettings = appSettings;
        }

        public T Get<T>(string key)
        {
            Migrate();

            return _storage.Get<T>(key);
        }

        public void Set<T>(string key, T value)
        {
            Migrate();

            _storage.Set(key, value);
        }

        public void RegisterMigration(IMigration migration)
        {
            _migrations.Add(migration);
        }

        private void Migrate()
        {
            if (_isMigrating)
                return;

            var version = Version;
            if (version == _appConfig.AppVersion)
                return;

            if (MigrationRequired(version))
            {
                _isMigrating = true;

                foreach (var migration in _migrations.OrderBy(m => m.ToVersion))
                {
                    migration.Apply();
                }
            }

            Version = _appConfig.AppVersion;

            _isMigrating = false;
        }

        private bool MigrationRequired(string version)
        {
            return !string.IsNullOrEmpty(version) || !UserSettingsMigrated;
        }

        private string Version
        {
            get => _storage.Get<string>(SettingsVersionKey);
            set => _storage.Set(SettingsVersionKey, value);
        }

        private bool UserSettingsMigrated => _appSettings.Get<bool>(UserSettingsMigratedKey);
    }
}
