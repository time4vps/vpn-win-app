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

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace T4VPN.Common.Configuration.Storage
{
    internal class ValidatedConfigStorage : IConfigStorage
    {
        private readonly IConfigStorage _origin;

        public ValidatedConfigStorage(IConfigStorage origin)
        {
            _origin = origin;
        }

        public Config Value()
        {
            var value = _origin.Value();
            return Valid(value) ? value : null;
        }

        public void Save(Config value)
        {
            _origin.Save(value);
        }

        private bool Valid(Config value)
        {
            return Valid((object) value) &&
                   Valid(value.OpenVpn) &&
                   Valid(value.Urls);
        }

        private bool Valid(object value)
        {
            if (value == null) return false;

            var context = new ValidationContext(value, null, null);
            var results = new List<ValidationResult>();

            return Validator.TryValidateObject(value, context, results, true);
        }
    }
}
