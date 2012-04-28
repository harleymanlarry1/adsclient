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
using Ads.Client.CommandResponse;
using Ads.Client.Common;

namespace Ads.Client.Commands
{
    public class AdsWriteReadCommand : AdsCommand
    {
        private uint readLength;
        private byte[] value;

        public AdsWriteReadCommand(uint indexGroup, uint indexOffset, byte[] value, uint readLength)
            : base(AdsCommandId.ReadWrite)
        {
            this.readLength = readLength;
            this.value = value;
            this.indexGroup = indexGroup;
            this.indexOffset = indexOffset;
        }

        private uint indexOffset;
        private uint indexGroup;

        internal override IEnumerable<byte> GetBytes()
        { 
            IEnumerable<byte> data = BitConverter.GetBytes(indexGroup);
            data = data.Concat(BitConverter.GetBytes(indexOffset));
            data = data.Concat(BitConverter.GetBytes(readLength));
            data = data.Concat(BitConverter.GetBytes((uint)value.Length));
            data = data.Concat(value);
            return data;
        }

        #if !NO_ASYNC
        public async Task<AdsWriteReadCommandResponse> RunAsync(Ams ams)
        {
            return await RunAsync<AdsWriteReadCommandResponse>(ams);
        }
        #endif

        #if !SILVERLIGHT
        public AdsWriteReadCommandResponse Run(Ams ams)
        {
            return Run<AdsWriteReadCommandResponse>(ams);
        }
        #endif
    }
}
