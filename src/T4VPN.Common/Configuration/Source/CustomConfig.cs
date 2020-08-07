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

using T4VPN.Common.Configuration.Storage;

namespace T4VPN.Common.Configuration.Source
{
    internal class CustomConfig : IConfigSource
    {
        private readonly ConfigMode _mode;
        private readonly IConfigSource _default;
        private readonly IConfigStorage _storage;

        public CustomConfig(ConfigMode mode, IConfigSource defaultSource, IConfigStorage storage)
        {
            _mode = mode;
            _default = defaultSource;
            _storage = storage;
        }

        public Config Value()
        {
            if (_mode == ConfigMode.Default)
            {
                return _default.Value();
            }
            
            var value = _storage.Value();
            if (value != null)
            {
                return value;
            }

            value = _default.Value();
            if (_mode == ConfigMode.CustomOrSavedDefault)
            {
                _storage.Save(value);
            }

            return value;
        }
    }
}
