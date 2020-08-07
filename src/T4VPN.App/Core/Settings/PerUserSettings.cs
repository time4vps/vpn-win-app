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

using System;
using T4VPN.Core.Auth;
using T4VPN.Core.Storage;
using System.Collections.Generic;
using System.Linq;
using T4VPN.Core.OS.Crypto;

namespace T4VPN.Core.Settings
{
    internal class PerUserSettings : ISettingsStorage, ILoggedInAware, ILogoutAware
    {
        private const string KeyPrefix = "User";
        private const string UserKey = "Username";
        private readonly ISettingsStorage _storage;
        private readonly Dictionary<string, object> _cache = new Dictionary<string, object>();
        private string _user = string.Empty;

        public PerUserSettings(ISettingsStorage storage)
        {
            _storage = storage;
        }

        public T Get<T>(string key)
        {
            if (_cache.TryGetValue(key, out object cachedValue))
                return cachedValue is T result ? result : default(T);

            var perUser = _storage.Get<PerUser<T>[]>(PerUserKey(key))?.SingleOrDefault(i => i.User == User);
            var value = perUser != null ? perUser.Value : default(T);

            _cache[key] = value;
            return value;
        }

        public void Set<T>(string key, T value)
        {
            _cache[key] = value;

            var all = _storage.Get<PerUser<T>[]>(PerUserKey(key))?.ToList() ?? new List<PerUser<T>>();
            var perUser = all.FirstOrDefault(i => i.User == User);
            if (perUser == null)
            {
                perUser = new PerUser<T> { User = User };
                all.Add(perUser);
            }
            perUser.Value = value;

            _storage.Set(PerUserKey(key), all);
        }

        public void OnUserLoggedIn()
        {
            _cache.Clear();
            _user = GetUser();
        }

        public void OnUserLoggedOut()
        {
            _cache.Clear();
            _user = string.Empty;
        }

        private string PerUserKey(string key) => KeyPrefix + key;

        private string User
        {
            get
            {
                if (!string.IsNullOrEmpty(_user))
                    return _user;

                _cache.Clear();
                return GetUser();
            }
        }

        private string GetUser()
        {
            var user = _storage.Get<string>(UserKey);
            if (string.IsNullOrEmpty(user))
                throw new InvalidOperationException("Access to user settings is not allowed: user is not logged in");

            return TrimDomain(user.Decrypt()).ToUpperInvariant();
        }

        private static string TrimDomain(string username)
        {
            if (username.IndexOf('@') is var i && i >= 0)
                return username.Substring(0, i);

            return username;
        }
    }

    internal class PerUser<T>
    {
        public string User { get; set; }
        public T Value { get; set; }
    }
}
