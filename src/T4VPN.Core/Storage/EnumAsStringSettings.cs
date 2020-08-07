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

namespace T4VPN.Core.Storage
{
    public class EnumAsStringSettings : ISettingsStorage
    {
        private readonly ISettingsStorage _storage;

        public EnumAsStringSettings(ISettingsStorage storage)
        {
            _storage = storage;
        }

        public T Get<T>(string key)
        {
            var toType = UnwrapNullable(typeof(T));
            if (toType.IsEnum)
            {
                var stringValue = _storage.Get<string>(key);
                TryParseEnum(stringValue, out T result);
                return result;
            }

            return _storage.Get<T>(key);
        }

        public void Set<T>(string key, T value)
        {
            var fromType = UnwrapNullable(typeof(T));
            if (fromType.IsEnum)
            {
                if (value != null)
                {
                    var intValue = Convert.ChangeType(value, Enum.GetUnderlyingType(fromType));
                    _storage.Set(key, intValue.ToString());
                    return;
                }

                _storage.Set(key, "");
            }
            else
            {
                _storage.Set(key, value);
            }
        }

        private bool TryParseEnum<T>(string value, out T result)
        {
            try
            {
                result = ParseEnum<T>(value);
                return true;
            }
            catch (InvalidCastException) { }
            catch (FormatException) { }

            result = default;
            return false;
        }

        private T ParseEnum<T>(string value)
        {
            var underlyingType = Enum.GetUnderlyingType(typeof(T));
            return (T)Convert.ChangeType(value, underlyingType);
        }

        private Type UnwrapNullable(Type type)
        {
            if (IsNullableType(type))
                return Nullable.GetUnderlyingType(type);

            return type;
        }

        private bool IsNullableType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
    }
}
