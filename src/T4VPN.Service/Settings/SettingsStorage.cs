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

using T4VPN.Common.Logging;
using T4VPN.Common.Storage;
using T4VPN.Common.Text.Serialization;
using T4VPN.Service.Contract.Settings;

namespace T4VPN.Service.Settings
{
    public class SettingsStorage
    {
        private readonly IStorage<SettingsContract> _origin;

        public SettingsStorage(ILogger logger, ITextSerializerFactory serializers, Common.Configuration.Config config)
        {
            _origin =
                new SafeStorage<SettingsContract>(
                    new LoggingStorage<SettingsContract>(
                        logger,
                        new FileStorage<SettingsContract>(
                            serializers,
                            config.ServiceSettingsFilePath)));
        }

        public SettingsContract Get()
        {
            return _origin.Get();
        }

        public void Set(SettingsContract value)
        {
            _origin.Set(value);
        }
    }
}
