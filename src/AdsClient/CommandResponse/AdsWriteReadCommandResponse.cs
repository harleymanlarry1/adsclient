﻿/*  This file is part of AdsClient created by Roeland Moors.

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

namespace Ads.Client.CommandResponse
{
    public class AdsWriteReadCommandResponse : AdsCommandResponse
    {
        private byte[] data;
        public byte[] Data
        {
            get { return data; }
        }


        protected override void AdsResponseIsChanged()
        {
            uint dataLength = BitConverter.ToUInt16(this.AdsResponse, 4);
            data = new byte[dataLength];
            Array.Copy(AdsResponse, 8, data, 0, (int)dataLength);
        }
    }
}
