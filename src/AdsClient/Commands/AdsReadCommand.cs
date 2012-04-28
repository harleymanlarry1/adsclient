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
using System.Threading.Tasks;
using Ads.Client.CommandResponse;
using Ads.Client.Common;

namespace Ads.Client.Commands
{
    internal class AdsReadCommand : AdsCommand
    {
        public AdsReadCommand(uint indexGroup, uint indexOffset, uint readLength)
            : base(AdsCommandId.Read)
        {
            this.readLength = readLength;
            this.indexGroup = indexGroup;
            this.indexOffset = indexOffset;
        }

        private uint readLength;
        private uint indexOffset;
        private uint indexGroup;

        internal override IEnumerable<byte> GetBytes()
        {
            IEnumerable<byte> data = BitConverter.GetBytes(indexGroup);
            data = data.Concat(BitConverter.GetBytes(indexOffset));
            data = data.Concat(BitConverter.GetBytes(readLength));

            return data;
        }

#if !NO_ASYNC
        public async Task<AdsReadCommandResponse> RunAsync(Ams ams) 
        {
            return await RunAsync<AdsReadCommandResponse>(ams);
        }
#endif

#if !SILVERLIGHT
        public AdsReadCommandResponse Run(Ams ams) 
        {
            return Run<AdsReadCommandResponse>(ams);
        }
#endif
    }
}
