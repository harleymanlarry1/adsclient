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

namespace Ads.Client.Common
{
    public class AmsNetId
    {
        public IList<byte> Bytes { get; set; }

        internal AmsNetId(string amsNetId)
        {
            ParseString(amsNetId);
        }

        private void ParseString(string amsNetId)
        {
            Bytes = new List<byte>();
            
            string[] byteStrings = amsNetId.Split('.');
            foreach (string byteString in byteStrings)
            {
                byte b = Convert.ToByte(byteString);
                Bytes.Add(b);
            }
        }
    }
}
