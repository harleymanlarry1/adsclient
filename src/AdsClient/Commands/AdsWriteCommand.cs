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
using System.Threading.Tasks;
using Ads.Client.CommandResponse;
using Ads.Client.Common;

namespace Ads.Client.Commands
{
    public class AdsWriteCommand : AdsCommand
    {
        private IEnumerable<byte> varValue;

        public AdsWriteCommand(uint indexGroup, uint indexOffset, IEnumerable<byte> varValue)
            : base(AdsCommandId.Write)
        {
            this.varValue = varValue;
            this.indexGroup = indexGroup;
            this.indexOffset = indexOffset;
        }

        private uint indexOffset;
        private uint indexGroup;

        internal override IEnumerable<byte> GetBytes()
        {
            IEnumerable<byte> data = BitConverter.GetBytes(indexGroup);
            data = data.Concat(BitConverter.GetBytes(indexOffset));
            data = data.Concat(BitConverter.GetBytes((uint)varValue.Count()));
            data = data.Concat(varValue);

            return data;
        }

        #if !NO_ASYNC
        public async Task<AdsWriteCommandResponse> RunAsync(Ams ams)
        {
            return await RunAsync<AdsWriteCommandResponse>(ams);
        }
        #endif

        #if !SILVERLIGHT
        public AdsWriteCommandResponse Run(Ams ams)
        {
            return Run<AdsWriteCommandResponse>(ams);
        }
        #endif
    }
}