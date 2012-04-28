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
using Ads.Client.Common;

namespace Ads.Client.CommandResponse
{
    public class AdsReadDeviceInfoCommandResponse : AdsCommandResponse
    {
        private AdsDeviceInfo adsDeviceInfo;
        public AdsDeviceInfo AdsDeviceInfo
        {
            get { return adsDeviceInfo; }
        }
        
        protected override void AdsResponseIsChanged()
        {
            adsDeviceInfo = new AdsDeviceInfo();
            adsDeviceInfo.MajorVersion = this.AdsResponse[4];
            adsDeviceInfo.MinorVersion = this.AdsResponse[5];
            adsDeviceInfo.VersionBuild = BitConverter.ToUInt16(this.AdsResponse, 6);
            adsDeviceInfo.DeviceName = BitConverter.ToString(this.AdsResponse, 8, 16);
        }
    }
}
