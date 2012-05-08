/*  Copyright (c) 2011 Roeland Moors
 
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
using System.Net.Sockets;
using Ads.Client.Helpers;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Ads.Client.Common;
using System.Diagnostics;

namespace Ads.Client
{
    public class AmsSocket : IDisposable, IAmsSocket
    {
        internal AmsSocket(string ipTarget, int ipPortTarget)
        {
            Subscribers = 0;
            LocalEndPoint = new IPEndPoint(IPAddress.Any, 0);
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.ipTarget = ipTarget;
            this.ipPortTarget = ipPortTarget;
            this.synchronizationContext = SynchronizationContext.Current;
        }

        private Socket socket { get; set; }
        private SynchronizationContext synchronizationContext;

        private string ipTarget;
        public string IpTarget { get { return ipTarget; } }

        private int ipPortTarget;
        public int IpPortTarget { get { return ipPortTarget; } }

        public IPEndPoint LocalEndPoint { get; set; }
        public int Subscribers { get; set; }

        public event AmsSocketResponseDelegate OnReadCallBack;

        private bool? connectedAsync;
        public bool? ConnectedAsync { get { return connectedAsync; } }

        public bool Connected { get { return socket.Connected; } }

        private void Listen()
        {
            if ((socket != null) && (socket.Connected))
            {
                try
                {
                    //First wait for the Ams header (starts new thread)
                    byte[] amsheader = new byte[AmsHeaderHelper.AmsTcpHeaderSize];
                   
                    SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                    args.SetBuffer(amsheader, 0, AmsHeaderHelper.AmsTcpHeaderSize);
                    args.UserToken = synchronizationContext;
                    args.Completed += (sender, e) => { 
                        //If a ams header is received, then read the rest (this is the new thread)
                        try
                        {
                            byte[] response = GetAmsMessage(e.Buffer);

                            Debug.WriteLine("Received bytes: " +
                                    ByteArrayHelper.ByteArrayToTestString(e.Buffer) + ',' +
                                    ByteArrayHelper.ByteArrayToTestString(response)); 
                            
                            var callbackArgs = new AmsSocketResponseArgs() { 
                                Response = response, 
                                Context = e.UserToken as SynchronizationContext };
                            OnReadCallBack(this, callbackArgs);
                            Listen();
                        }
                        catch (Exception ex)
                        {
                            var callbackArgs = new AmsSocketResponseArgs() { Error = ex };
                            OnReadCallBack(this, callbackArgs);
                        }

                    };
                    if (!socket.ReceiveAsync(args))
                    {
                        //If finished in same thread
                        byte[] response = GetAmsMessage(amsheader);
                        var callbackArgs = new AmsSocketResponseArgs()
                        {
                            Response = response,
                            Context = synchronizationContext
                        };
                        OnReadCallBack(this, callbackArgs);
                        Listen();
                    };
                     
                }
                catch (Exception ex)
                {
                    if (!Object.ReferenceEquals(ex.GetType(), typeof(ObjectDisposedException))) throw;
                }
            }
        }

        private byte[] GetAmsMessage(byte[] tcpHeader)
        {
            uint responseLength = AmsHeaderHelper.GetResponseLength(tcpHeader);
            byte[] response = new byte[responseLength];
            GetMessage(response);
            return response;
        }

        private void GetMessage(byte[] response)
        {
            #if SILVERLIGHT
            ReceiveAsync(response).Wait(); 
            #else
            Receive(response);
            #endif
        }

        private void CloseConnection()
        {
            if (socket.Connected)
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }

            if (socket != null) socket.Dispose();
            
        }

        protected virtual void Dispose(bool managed)
        {
            CloseConnection();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #region Async Methods
        #if !NO_ASYNC
        public async Task ConnectAndListenAsync()
        {
            if (!socket.Connected)
            {
                connectedAsync = true;
                await ConnectAsync();
                Listen();
            }
        }

        private async Task<bool> ConnectAsync()
        {
            var tcs = new TaskCompletionSource<bool>(socket);
            #if !SILVERLIGHT
            socket.Bind(LocalEndPoint);
            #endif
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.RemoteEndPoint = new DnsEndPoint(ipTarget, ipPortTarget);
            args.Completed += (sender, e) => { tcs.TrySetResult(e.SocketError == SocketError.Success); };
            socket.ConnectAsync(args);
            return await tcs.Task;
        }

        public async Task<bool> SendAsync(byte[] message)
        {
            var tcs = new TaskCompletionSource<bool>(socket);
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += (sender, e) => { tcs.TrySetResult(e.SocketError == SocketError.Success); };
            args.SetBuffer(message, 0, message.Length);
            socket.SendAsync(args);
            return await tcs.Task;
        }

        private async Task<bool> ReceiveAsync(byte[] message)
        {
            var tcs = new TaskCompletionSource<bool>(socket);
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += (sender, e) =>
            {
                try { tcs.TrySetResult(e.SocketError == SocketError.Success); }
                catch (Exception ex) { tcs.TrySetException(ex); }
            };
            args.SetBuffer(message, 0, message.Length);
            try
            {
                socket.ReceiveAsync(args);
            }
            catch (Exception ex)
            {
                if (!Object.ReferenceEquals(ex.GetType(), typeof(ObjectDisposedException))) throw ex;
            }
            return await tcs.Task;
        }

        #endif
        #endregion

        #region Blocking methods
        #if !SILVERLIGHT

        private int maxPacketSize = 1514;

        public void ConnectAndListen()
        {
            if (!socket.Connected)
            {
                connectedAsync = false;
                Connect();
                Listen();
            }
        }

        private void Connect()
        {
            socket.Bind(LocalEndPoint);
            socket.Connect(ipTarget, ipPortTarget);
        }

        public void Send(byte[] message)
        {
            Debug.WriteLine("Sending bytes: " + ByteArrayHelper.ByteArrayToTestString(message)); 
            socket.Send(message);
        }

        public void Receive(byte[] message)
        {
            try
            {
                if (message.Length <= maxPacketSize)
                {
                    socket.Receive(message);
                }
                else
                {
                    byte[] msg = new byte[maxPacketSize];
                    int totalread = 0;

                    while (totalread < message.Length)
                    {
                        int read = socket.Receive(msg);
                        Array.Copy(msg, 0, message, totalread, read);
                        totalread += read;
                    }
                }
            }
            catch (Exception ex)
            {
                if (!Object.ReferenceEquals(ex.GetType(), typeof(ObjectDisposedException))) throw;
            }
        }

        #endif
        #endregion

    }
}