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
using T4VPN.BugReporting.Attachments.Source;
using T4VPN.Common.Helpers;
using T4VPN.Common.Logging;
using Module = Autofac.Module;

namespace T4VPN.BugReporting
{
    public class BugReportingModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<BugReport>().SingleInstance();

            builder.Register(c =>
            {
                var appConfig = c.Resolve<Common.Configuration.Config>();
                var logger = c.Resolve<ILogger>();

                return new Attachments.Attachments(
                    logger,
                    appConfig,
                    new FilesToAttachments(
                        new ConcatenatedSequence<string>(
                            new SafeFileSource(logger,
                                new LogFileSource(appConfig.AppLogFolder, appConfig.MaxAppLogsAttached)),
                            new SafeFileSource(logger,
                                new LogFileSource(appConfig.ServiceLogFolder, appConfig.MaxServiceLogsAttached)),
                            new SafeFileSource(logger,
                                new LogFileSource(appConfig.UpdateServiceLogFolder, appConfig.MaxUpdaterServiceLogsAttached)))),
                    new FilesToAttachments(
                        new SelectFileSource()));
            }).SingleInstance();
        }
    }
}
