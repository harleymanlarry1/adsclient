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

namespace Ads.Client.Helpers
{
    internal class AmsHeaderHelper
    {
        public const int AmsTcpHeaderSize = 6;

        public static uint GetResponseLength(byte[] tcpHeader)
        {
            return BitConverter.ToUInt32(tcpHeader, 2);
        }

        public static byte[] GetAmsNetIdTarget(byte[] amsHeader)
        {
            byte[] id = new byte[6];
            Array.Copy(amsHeader, 0, id, 0, 6);
            return id;
        }

        public static ushort GetAmsPortTarget(byte[] amsHeader)
        {
            return BitConverter.ToUInt16(amsHeader, 14);
        }

        public static byte[] GetAmsNetIdSource(byte[] amsHeader)
        {
            byte[] id = new byte[6];
            Array.Copy(amsHeader, 8, id, 0, 6);
            return id;
        }

        public static ushort GetAmsPortSource(byte[] amsHeader)
        {
            return BitConverter.ToUInt16(amsHeader, 14);
        }

        public static ushort GetCommandId(byte[] amsHeader)
        {
            return BitConverter.ToUInt16(amsHeader, 16);
        }

        public static uint GetErrorCode(byte[] amsHeader)
        {
            return BitConverter.ToUInt32(amsHeader, 24);
        }

        public static uint GetInvokeId(byte[] amsHeader)
        {
            return BitConverter.ToUInt32(amsHeader, 28);
        }

    }
}
