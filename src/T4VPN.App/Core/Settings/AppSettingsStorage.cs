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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using T4VPN.Common.Extensions;
using T4VPN.Common.Logging;
using T4VPN.Core.Abstract;
using T4VPN.Core.Storage;

namespace T4VPN.Core.Settings
{
    internal class AppSettingsStorage : ISettingsStorage, ISupportsMigration
    {
        private readonly object _saveLock = new object();
        private readonly List<IMigration> _migrations = new List<IMigration>();
        private readonly ILogger _logger;

        private bool _loaded;

        public AppSettingsStorage(ILogger logger)
        {
            _logger = logger;
        }

        public T Get<T>(string key)
        {
            return (T) Properties.Settings.Default[key];
        }

        public void Set<T>(string key, T value)
        {
            Properties.Settings.Default[key] = value;
            Save();
        }

        public void RegisterMigration(IMigration migration)
        {
            _migrations.Add(migration);
        }

        public void Load()
        {
            if (_loaded)
                return;

            if (Properties.Settings.Default.AppFirstRun)
            {
                Migrate();
                ExecuteCustomMigrations();
                Save();
            }

            _loaded = true;
        }

        private void Save()
        {
            lock (_saveLock)
            {
                try
                {
                    Properties.Settings.Default.Save();
                }
                catch (Exception e) when (IsSettingsSavingException(e))
                {
                    _logger.Error("Failed to save app settings: " + e.Message);
                }
            }
        }

        private bool IsSettingsSavingException(Exception e)
        {
            return e.IsFileAccessException() ||
                   e is ArgumentException ||
                   e is ConfigurationErrorsException;
        }

        private void Migrate()
        {
            try
            {
                Properties.Settings.Default.Upgrade();
            }
            catch (Win32Exception)
            {
            }
            catch (IOException)
            {
            }
            catch (ConfigurationException)
            {
            }

            Properties.Settings.Default.AppFirstRun = false;
            Properties.Settings.Default.LastUpdate = DateTime.Now.ToShortDateString();
        }

        private void ExecuteCustomMigrations()
        {
            foreach (var migration in _migrations.OrderBy(m => m.ToVersion))
            {
                migration.Apply();
            }
        }
    }
}
