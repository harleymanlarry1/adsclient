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
    public class AdsDeleteDeviceNotificationCommand : AdsCommand
    {
        public AdsDeleteDeviceNotificationCommand(uint notificationHandle) 
            : base(AdsCommandId.DeleteDeviceNotification)
        {
           
            this.notificationHandle = notificationHandle;
        }

        private uint notificationHandle;

        internal override IEnumerable<byte> GetBytes()
        {
            IEnumerable<byte> data = BitConverter.GetBytes(notificationHandle);
            return data;
        }

        protected override void RunAfter(Ams ams)
        {
            var notification = ams.NotificationRequests.FirstOrDefault(n => n.NotificationHandle == notificationHandle);
            if (notification != null)
                ams.NotificationRequests.Remove(notification);
        }

        #if !NO_ASYNC
        public async Task<AdsDeleteDeviceNotificationCommandResponse> RunAsync(Ams ams)
        {
            return await RunAsync<AdsDeleteDeviceNotificationCommandResponse>(ams);
        }
        #endif

        #if !SILVERLIGHT
        public AdsDeleteDeviceNotificationCommandResponse Run(Ams ams)
        {
            return Run<AdsDeleteDeviceNotificationCommandResponse>(ams);
        }
        #endif
    }
}
