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
using System.Threading.Tasks;
using Ads.Client.CommandResponse;
using Ads.Client.Common;

namespace Ads.Client.Commands
{
    public abstract class AdsCommand
    {
        public AdsCommand(ushort commandId)
        {
            this.commandId = commandId;
        }

        
        private ushort commandId;
        public ushort CommandId
        {
            get { return commandId; }
        }

        protected virtual void RunBefore(Ams ams)
        {
        }

        protected virtual void RunAfter(Ams ams)
        {
        }

        #if !NO_ASYNC
        protected async Task<T> RunAsync<T>(Ams ams) where T : AdsCommandResponse
        {
            RunBefore(ams);
            var result = await ams.RunCommandAsync<T>(this);
            if (result.AdsErrorCode > 0)
                throw new AdsException(result.AdsErrorCode);
            RunAfter(ams);
            return result;
        }
        #endif


        #if !SILVERLIGHT
        protected T Run<T>(Ams ams) where T : AdsCommandResponse
        {
            RunBefore(ams);
            var result = ams.RunCommand<T>(this);
            if (result.AdsErrorCode > 0)
                throw new AdsException(result.AdsErrorCode);
            RunAfter(ams);
            return result;
        }
        #endif

        internal abstract IEnumerable<byte> GetBytes();
    }
}
