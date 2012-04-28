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
    public static class AdsCommandId 
    {
        public const ushort ReadDeviceInfo             = 1;
        public const ushort Read                       = 2;
        public const ushort Write                      = 3;
        public const ushort ReadState                  = 4;
        public const ushort WriteControl               = 5;
        public const ushort AddDeviceNotification      = 6;
        public const ushort DeleteDeviceNotification   = 7;
        public const ushort DeviceNotification         = 8;
        public const ushort ReadWrite                  = 9;
    }
}
