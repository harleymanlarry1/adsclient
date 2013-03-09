﻿/*  Copyright (c) 2011 Roeland Moors
 
    Permission is hereby granted, free of charge, to any person obtaining a 
    copy of this software and associated documentation files (the "Software"), 
    to deal in the Software without restriction, including without limitation 
    the rights to use, copy, modify, merge, publish, distribute, sublicense, 
    and/or sell copies of the Software, and to permit persons to whom the 
    Software is furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included 
    in all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
    EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
    MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
    IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, 
    DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
    TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE 
    OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using Ads.Client.Commands;
using Ads.Client.Common;
using Ads.Client.CommandResponse;
using Ads.Client.Helpers;
using System.Threading;

namespace Ads.Client
{

    public class Ams : IDisposable
    {

        internal Ams(string ipTarget, int ipPortTarget = 48898)
        {
            this.IpTarget = ipTarget;
            this.NotificationRequests = new List<AdsNotification>();
            this.amsSocket = AmsSocketHelper.GetOrCreateAmsSocket(ipTarget, ipPortTarget);
            this.amsSocket.OnReadCallBack += new AmsSocketResponseDelegate(ReadCallback);
        }

        internal Ams(IAmsSocket amsSocket)
        {
            this.amsSocket = amsSocket;
            this.amsSocket.OnReadCallBack += new AmsSocketResponseDelegate(ReadCallback);
        }

        /// <summary>
        /// Ams source port. Default of 32905
        /// </summary>
        private ushort amsPortSource = 32905;
        public ushort AmsPortSource
        {
            get { return amsPortSource; }
            set { amsPortSource = value; }
        }        

        /// <summary>
        /// The localendpoint for the socket connection
        /// </summary>
        public IPEndPoint LocalEndPoint
        {
            get { return amsSocket.LocalEndPoint; }
            set { amsSocket.LocalEndPoint = value; }
        }

        /// <summary>
        /// Specify a timeout time for the non async functions.
        /// The default is 5000 ms. 
        /// -1 means wait forever
        /// </summary>
        private int commandTimeOut = 5000;
        public int CommandTimeOut
        {
            get { return commandTimeOut; }
            set { commandTimeOut = value; }
        }

        /// <summary>
        /// Run the notifications in the main thread.
        /// This only works when there is a SynchronizationContext available
        /// </summary>
        public bool RunNotificationsOnMainThread { get; set; }

        internal string IpTarget { get; set; }
        internal ushort AmsPortTarget { get; set; }
        internal AmsNetId AmsNetIdTarget { get; set; }
        internal AmsNetId AmsNetIdSource { get; set; }
        internal List<AdsNotification> NotificationRequests;
        internal bool? ConnectedAsync { get { return amsSocket.ConnectedAsync; } }

        private IAmsSocket amsSocket;
        private uint invokeId = 0;
        private List<AdsCommandResponse> pendingResults = new List<AdsCommandResponse>();

        private byte[] GetAmsMessage(AdsCommand adsCommand) 
        {
            IEnumerable<byte> data = adsCommand.GetBytes();
            IEnumerable<byte> message = AmsNetIdTarget.Bytes;                       //AmsNetId Target
            message = message.Concat(BitConverter.GetBytes(AmsPortTarget));         //AMSPort Target
            message = message.Concat(AmsNetIdSource.Bytes);                         //AmsNetId Source
            message = message.Concat(BitConverter.GetBytes(AmsPortSource));         //AMSPort Source
            message = message.Concat(BitConverter.GetBytes(adsCommand.CommandId));  //Command Id
            message = message.Concat(BitConverter.GetBytes((ushort)0x0004));        //State Flags
            message = message.Concat(BitConverter.GetBytes((uint)data.Count()));    //Length
            message = message.Concat(BitConverter.GetBytes((uint)0));               //Error Code
            message = message.Concat(BitConverter.GetBytes(invokeId));              //Invoke Id
            message = message.Concat(data);                                         //Data

            //2 bytes reserved 0 + 4 bytes for length + the rest
            message = BitConverter.GetBytes((ushort)0).Concat(BitConverter.GetBytes((uint)message.Count())).Concat(message);

            return message.ToArray<byte>();
        }

        internal bool Connected
        {
            get { return (amsSocket.Connected); }
        }

        //This is different thread!
        private void ReadCallback(object sender, AmsSocketResponseArgs args)
        {
            if (args.Error != null)
            {
                if ((pendingResults != null) && (pendingResults.Count() > 0))
                {
                    foreach (var adsCommandResult in pendingResults)
                    {
                        adsCommandResult.UnknownException = args.Error;
                        adsCommandResult.Callback.Invoke(adsCommandResult);
                    }
                }
                else throw args.Error;
            }

            if ((args.Response != null) && (args.Response.Length > 0) && (args.Error == null))
            {
                uint amsErrorCode = AmsHeaderHelper.GetErrorCode(args.Response);
                uint invokeId = AmsHeaderHelper.GetInvokeId(args.Response);
                bool isNotification = (AmsHeaderHelper.GetCommandId(args.Response) == AdsCommandId.DeviceNotification);

                if (AmsPortTarget != AmsHeaderHelper.GetAmsPortSource(args.Response)) return;
                if (!AmsNetIdTarget.Bytes.SequenceEqual(AmsHeaderHelper.GetAmsNetIdSource(args.Response))) return;

                //If notification then just start the callback
                if (isNotification && (OnNotification != null))
                {
                    var notifications = AdsNotification.GetNotifications(args.Response);
                    foreach (var notification in notifications)
                    {
                        var notificationRequest = NotificationRequests.FirstOrDefault(n => n.NotificationHandle == notification.NotificationHandle);
                        if (notificationRequest != null) 
                        {
                            notificationRequest.ByteValue = notification.ByteValue;

                            if ((args.Context != null) && (RunNotificationsOnMainThread))
                                args.Context.Post(
                                    new SendOrPostCallback(delegate 
                                    { 
                                        OnNotification(null, new AdsNotificationArgs(notificationRequest)); 
                                    }), null);
                            else
                                OnNotification(null, new AdsNotificationArgs(notificationRequest));
                        }
                    }
                }

                //If not a notification then find the original command and call async callback
                if (!isNotification)
                {
                    AdsCommandResponse adsCommandResult = pendingResults.FirstOrDefault(r => r.CommandInvokeId == invokeId);
                    if (adsCommandResult != null) 
                    {
                        pendingResults.Remove(adsCommandResult);
                        if (amsErrorCode > 0) 
                            adsCommandResult.AmsErrorCode = amsErrorCode;
                        else
                            adsCommandResult.SetResponse(args.Response);
                        adsCommandResult.Callback.Invoke(adsCommandResult);
                    }
                    else throw new AdsException("I received a response from a request I didn't send?");
                }
            }
        }

        private T EndGetResponse<T>(IAsyncResult ar) where T : AdsCommandResponse
        {
            return (T)ar;
        }

        private IAsyncResult BeginGetResponse<T>(AsyncCallback callback, object state) where T : AdsCommandResponse
        {
            uint invokeId = (state as uint?).Value;
            var result = Activator.CreateInstance<T>();
            result.CommandInvokeId = invokeId;
            result.Callback = callback;
            pendingResults.Add(result);
            return result;
        }

        internal event AdsNotificationDelegate OnNotification;

        protected virtual void Dispose(bool managed)
        {
            AmsSocketHelper.UnsibscribeAmsSocket(IpTarget);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #if !NO_ASYNC

        internal async Task<T> RunCommandAsync<T>(AdsCommand adsCommand) where T : AdsCommandResponse
        {
            await this.amsSocket.ConnectAndListenAsync();
            if (ConnectedAsync == false) throw new AdsException("You are combining async and non-async methods!");
            invokeId++;
            byte[] message = GetAmsMessage(adsCommand);
            var responseTask = Task.Factory.FromAsync<T>(BeginGetResponse<T>, EndGetResponse<T>, invokeId);
            await amsSocket.SendAsync(message);
            return await responseTask;
        }
        #endif

        #if !SILVERLIGHT

        internal T RunCommand<T>(AdsCommand adsCommand) where T : AdsCommandResponse
        {
            this.amsSocket.ConnectAndListen();
            if (ConnectedAsync == true) throw new AdsException("You are combining async and non-async methods!");
            invokeId++;
            byte[] message = GetAmsMessage(adsCommand);
            var task = Task.Factory.FromAsync<T>(BeginGetResponse<T>, EndGetResponse<T>, invokeId);
            amsSocket.Send(message);
            if (!task.Wait(CommandTimeOut)) throw new AdsException(String.Format("Running the command timed out after {0} ms!", CommandTimeOut));
            if (task.Result.UnknownException != null) throw task.Result.UnknownException;
            return task.Result;
        }
        #endif



    }
}
