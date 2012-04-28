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
using System.Net.Sockets;
using Ads.Client.Helpers;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Ads.Client.Common;

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

        public bool Verbose { get; set; }

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

                            if (Verbose)
                                Console.WriteLine("Received bytes: " +
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
            if (Verbose)
                Console.WriteLine("Sending bytes: " + ByteArrayHelper.ByteArrayToTestString(message)); 
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