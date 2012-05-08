using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Ads.Client.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ads.Client.Helpers;
using System.Diagnostics;

namespace Ads.Client.Test
{
    class AmsSocketTest : IAmsSocket
    {
        public bool Connected { get; set; }
        public bool? ConnectedAsync { get; set; }
        public IPEndPoint LocalEndPoint { get; set; }
        public byte[] SendMessage { get; set; }
        public byte[] ReceiveMessage { get; set; }
        public bool Verbose { get; set; }
        public event AmsSocketResponseDelegate OnReadCallBack;

        public void ConnectAndListen() 
        {
            ConnectedAsync = false;
        }

        public void Send(byte[] message)
        {
            Trace.WriteLine("message: " + ByteArrayHelper.ByteArrayToTestString(message));
            Trace.WriteLine("sendmsg: " + ByteArrayHelper.ByteArrayToTestString(SendMessage));
            Assert.IsTrue(message.SequenceEqual(SendMessage));
            int length = ReceiveMessage.Length-6;
            byte[] response = new byte[length];
            Array.Copy(ReceiveMessage, 6, response, 0, length);
            var callbackArgs = new AmsSocketResponseArgs()
            {
                Response = response
            };
            OnReadCallBack(this, callbackArgs);
        }

        public async Task ConnectAndListenAsync()
        {
            ConnectedAsync = true;
        }

        public async Task<bool> SendAsync(byte[] message)
        {
            Send(message);
            return await TaskEx.FromResult(true);
        }
    }
}
