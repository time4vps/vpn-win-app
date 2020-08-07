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
using System.Globalization;
using System.Windows.Data;
using ByteSizeLib;

namespace T4VPN.Core.MVVM.Converters
{
    internal class BytesToSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var bytes = ByteSize.FromBytes((double?)value ?? 0.0);
            var size = bytes.LargestWholeNumberDecimalValue;

            var format = "0";
            if (bytes.Bytes >= ByteSize.BytesInKiloByte)
            {
                if (size < 10.0)
                {
                    format = "0.00";
                }
                else if (size < 100.0)
                {
                    format = "0.0";
                }
            }

            return size.ToString(format);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}