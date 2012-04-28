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
    public class AdsReadDeviceInfoCommand : AdsCommand
    {

        public AdsReadDeviceInfoCommand()
            : base(AdsCommandId.ReadDeviceInfo)
        {
            
        }

        internal override IEnumerable<byte> GetBytes()
        {
            return new List<byte>();
        }

        #if !NO_ASYNC
        public async Task<AdsReadDeviceInfoCommandResponse> RunAsync(Ams ams)
        {
            return await RunAsync<AdsReadDeviceInfoCommandResponse>(ams);
        }
        #endif

        #if !SILVERLIGHT
        public AdsReadDeviceInfoCommandResponse Run(Ams ams)
        {
            return Run<AdsReadDeviceInfoCommandResponse>(ams);
        }
        #endif
    }
}
