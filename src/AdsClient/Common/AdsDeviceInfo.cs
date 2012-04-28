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

namespace Ads.Client.Common
{
    public class AdsDeviceInfo
    {
        public byte MajorVersion { get; set; }
        public byte MinorVersion { get; set; }
        public ushort VersionBuild { get; set; }
        public string DeviceName { get; set; }

        public override string ToString()
        {
            return String.Format("Version: {0}.{1}.{2} Devicename: {3}", MajorVersion, MinorVersion, VersionBuild, DeviceName);
        }

    }
}
