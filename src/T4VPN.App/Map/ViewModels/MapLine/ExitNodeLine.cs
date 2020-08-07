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
    public class ExitNodeLine : MapLine
    {
        private readonly AbstractPinViewModel _start;
        private readonly AbstractPinViewModel _end;

        public string EntryNodeCountry { get; }
        public string ExitNodeCountry { get; }

        public ExitNodeLine(AbstractPinViewModel start, AbstractPinViewModel end)
        {
            _start = start;
            _end = end;
            EntryNodeCountry = start.CountryCode;
            ExitNodeCountry = end.CountryCode;
            SetPoints(1);
        }

        private double GetHorizontalOffset(AbstractPinViewModel pin)
        {
            return pin.HorizontalOffset + pin.Width / 2;
        }

        private double GetVerticalOffset(AbstractPinViewModel pin, double scale)
        {
            var offset = pin.VerticalOffset + pin.Height - 2 / scale;
            if (pin is SecureCorePinViewModel sc)
            {
                offset -= sc.PinHeight / scale / 2;
            }

            return offset;
        }

        public override void ApplyMapScale(double scale)
        {
            SetPoints(scale);
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
