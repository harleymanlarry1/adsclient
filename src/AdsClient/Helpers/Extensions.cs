/*  This file is part of AdsClient created by Roeland Moors.

    AdsClient is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    AdsClient is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with AdsClient.  If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ads.Client.Common;

namespace Ads.Client
{
    public static class Extensions
    {
        public static byte[] ToAdsBytes(this String value)
        {
            byte[] result = new byte[value.Length + 1];

            for (int index = 0; index < value.Length; ++index)
            {
                char ch = value[index];
                if (ch <= 0x7f) 
                    result[index] = (byte)ch;
                else 
                    result[index] = (byte)'?';
            }
            result[result.Length-1] = 0;

            return result;
        }

        public static Time ToTime(this TimeSpan value)
        {
            return new Time(value);
        }

        public static Date ToDate(this DateTime value)
        {
            return new Date(value);
        }
    }
}
