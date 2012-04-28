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
    public class AdsAddDeviceNotificationCommand : AdsCommand
    {
        public AdsAddDeviceNotificationCommand(uint indexGroup, uint indexOffset, uint readLength, AdsTransmissionMode transmissionMode)
            : base(AdsCommandId.AddDeviceNotification)
        {
            this.readLength = readLength;
            this.indexGroup = indexGroup;
            this.indexOffset = indexOffset;
            this.transmissionMode = transmissionMode;
            this.notification = new AdsNotification();
       
        }

        private AdsNotification notification;
        public AdsNotification Notification
        {
            get { return notification; }
        }

        public object UserData
        {
            get { return notification.UserData; }
            set { notification.UserData = value; }
        }

        public Type TypeOfValue
        {
            get { return notification.TypeOfValue; }
            set { notification.TypeOfValue = value; }
        }

        private AdsTransmissionMode transmissionMode;
        private uint readLength;
        private uint indexOffset;
        private uint indexGroup;

        public uint MaxDelay { get; set; }

        private uint cycleTime;
        public uint CycleTime
        {
            get { return cycleTime/10000; }
            set { cycleTime = value*10000; }
        }

        internal override IEnumerable<byte> GetBytes()
        {
            IEnumerable<byte> data = BitConverter.GetBytes(indexGroup);
            data = data.Concat(BitConverter.GetBytes(indexOffset));
            data = data.Concat(BitConverter.GetBytes(readLength));
            data = data.Concat(BitConverter.GetBytes((uint)transmissionMode));
            data = data.Concat(BitConverter.GetBytes(MaxDelay));
            data = data.Concat(BitConverter.GetBytes(cycleTime));
            data = data.Concat(BitConverter.GetBytes((UInt64)0));
            data = data.Concat(BitConverter.GetBytes((UInt64)0));
            return data;
        }

        protected override void RunBefore(Ams ams)
        {
            ams.NotificationRequests.Add(Notification);
        }

        #if !NO_ASYNC
        public async Task<AdsAddDeviceNotificationCommandResponse> RunAsync(Ams ams)
        {
            return await RunAsync<AdsAddDeviceNotificationCommandResponse>(ams);
        }
        #endif

        #if !SILVERLIGHT
        public AdsAddDeviceNotificationCommandResponse Run(Ams ams)
        {
            return Run<AdsAddDeviceNotificationCommandResponse>(ams);
        }
        #endif

    }
}
