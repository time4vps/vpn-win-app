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

using T4VPN.Map.ViewModels.Pins;

namespace T4VPN.Map.ViewModels.MapLine
{
    internal class SecureCoreLine : MapLine
    {
        private readonly SecureCorePinViewModel _start;
        private readonly SecureCorePinViewModel _end;

        public SecureCoreLine(SecureCorePinViewModel start, SecureCorePinViewModel end)
        {
            _start = start;
            _end = end;
            SetPoints(1);
        }

        public override void ApplyMapScale(double scale)
        {
            SetPoints(scale);
        }

        private double GetHorizontalOffset(SecureCorePinViewModel pin)
        {
            return pin.HorizontalOffset + pin.Width / 2;
        }

        private double GetVerticalOffset(SecureCorePinViewModel pin, double scale)
        {
            return pin.VerticalOffset + pin.Height - pin.PinHeight / scale / 2;
        }

        private void SetPoints(double scale)
        {
            X1 = GetHorizontalOffset(_start);
            Y1 = GetVerticalOffset(_start, scale);
            X2 = GetHorizontalOffset(_end);
            Y2 = GetVerticalOffset(_end, scale);
        }
    }
}
