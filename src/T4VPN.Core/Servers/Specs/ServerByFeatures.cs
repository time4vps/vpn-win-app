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

using T4VPN.Core.Abstract;
using T4VPN.Core.Api.Contracts;

namespace T4VPN.Core.Servers.Specs
{
    public class ServerByFeatures : Specification<LogicalServerContract>
    {
        private readonly Specification<LogicalServerContract> _spec;

        public ServerByFeatures(Features features)
        {
            _spec = SpecByFeatures(features);
        }

        public override bool IsSatisfiedBy(LogicalServerContract item)
        {
            return _spec.IsSatisfiedBy(item);
        }

        private static Specification<LogicalServerContract> SpecByFeatures(Features features)
        {
            var spec = (features.IsSecureCore() ? (Specification<LogicalServerContract>) new SecureCoreServer() : new StandardServer()) &
                       (features.SupportsTor() ? new TorServer() : !new TorServer());

            if (features.SupportsP2P())
                spec &= new P2PServer();

            return spec;
        }
    }
}
