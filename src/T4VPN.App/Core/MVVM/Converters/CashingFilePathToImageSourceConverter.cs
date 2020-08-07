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

using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace T4VPN.Core.MVVM.Converters
{
    internal class CachingFilePathToImageSourceConverter : IValueConverter
    {
        private readonly FilePathToImageSourceConverter _converter;

        private static ConcurrentDictionary<string, ImageSource> _images = new ConcurrentDictionary<string, ImageSource>();

        public CachingFilePathToImageSourceConverter()
        {
            _converter = new FilePathToImageSourceConverter();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var path = value as string;
            if (string.IsNullOrEmpty(path))
                return null;

            return _images.GetOrAdd(path, (p) => _converter.Convert(p, targetType, parameter, culture) as ImageSource);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
