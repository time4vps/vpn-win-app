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

using Autofac;
using T4VPN.Common.Logging;
using T4VPN.Update.Updates;

namespace T4VPN.Update.Config
{
    /// <summary>
    /// Initializes Update module and registers public interfaces.
    /// </summary>
    public class Module
    {
        public void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AppUpdates>().SingleInstance();

            builder.Register(c => 
                new CleanableOnceAppUpdates(
                    new AsyncAppUpdates(
                        new SafeAppUpdates(c.Resolve<ILogger>(),
                            c.Resolve<AppUpdates>())
                ))).As<IAppUpdates>().SingleInstance();

            builder.Register(c =>
                new SafeAppUpdate(c.Resolve<ILogger>(),
                    new ExtendedProgressAppUpdate(c.Resolve<IAppUpdateConfig>().MinProgressDuration,
                        new NotifyingAppUpdate(
                            new AppUpdate(c.Resolve<AppUpdates>())
                )))).As<INotifyingAppUpdate>().SingleInstance();
        }
    }
}
