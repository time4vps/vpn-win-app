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

namespace T4VPN.Core.Storage
{
    public class CachedSettings : ISettingsStorage
    {
        private readonly ISettingsStorage _storage;
        private readonly Dictionary<string, object> _cache = new Dictionary<string, object>();

        public CachedSettings(ISettingsStorage storage)
        {
            _storage = storage;
        }

        public T Get<T>(string key)
        {
            if (_cache.TryGetValue(key, out object cachedValue))
                return cachedValue is T result ? result : default;

            var value = _storage.Get<T>(key);
            _cache[key] = value;
            return value;
        }

        public void Set<T>(string key, T value)
        {
            _storage.Set(key, value);
            _cache[key] = value;
        }
    }
}
