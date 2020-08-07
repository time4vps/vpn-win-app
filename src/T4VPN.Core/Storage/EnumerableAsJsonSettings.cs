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

using Newtonsoft.Json;
using System;
using System.Collections;

namespace T4VPN.Core.Storage
{
    public class EnumerableAsJsonSettings : ISettingsStorage
    {
        private readonly ISettingsStorage _storage;

        public EnumerableAsJsonSettings(ISettingsStorage storage)
        {
            _storage = storage;
        }

        public T Get<T>(string key)
        {
            if (IsEnumerableType(typeof(T)))
            {
                var stringValue = _storage.Get<string>(key);
                return JsonConvert.DeserializeObject<T>(stringValue);
            }

            return _storage.Get<T>(key);
        }

        public void Set<T>(string key, T value)
        {
            if (IsEnumerableType(typeof(T)))
            {
                var stringValue = string.Empty;
                if (value != null)
                {
                    stringValue = JsonConvert.SerializeObject(value);
                }
                _storage.Set(key, stringValue);
            }
            else
            {
                _storage.Set(key, value);
            }
        }

        private bool IsEnumerableType(Type type)
        {
            return typeof(IEnumerable).IsAssignableFrom(type) && type != typeof(string);
        }
    }
}
