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

namespace T4VPN.Core.Api.Contracts
{
    public class Location : BaseResponse
    {
        private float _lat;
        public float Lat
        {
            get => _lat;
            set
            {
                if (value > 90) { _lat = 90; }
                else if (value < -90) { _lat = -90; }
                else { _lat = value; }
            }
        }

        private float _longitude;
        public float Long
        {
            get => _longitude;
            set
            {
                if (value > 180) { _longitude = 180; }
                else if (value < -180) { _longitude = -180; }
                else { _longitude = value; }
            }
        }
    }
}
