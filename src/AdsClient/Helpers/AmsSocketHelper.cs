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
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Ads.Client;

namespace Ads.Client.Helpers
{
    internal class AmsSocketHelper
    {
        static AmsSocketHelper()
        {
            SocketList = new List<AmsSocket>();
        }

        private static List<AmsSocket> SocketList;

        public static AmsSocket GetOrCreateAmsSocket(string ipTarget, int ipPortTarget)
        {
            AmsSocket amsSocket = SocketList.FirstOrDefault(s => (s.IpTarget == ipTarget) && (s.IpPortTarget == ipPortTarget));
            if (amsSocket == null)
            {
                amsSocket = new AmsSocket(ipTarget, ipPortTarget);
                SocketList.Add(amsSocket);
            }
            amsSocket.Subscribers++;
            return amsSocket;
        }

        public static void UnsibscribeAmsSocket(string ipTarget)
        {
            AmsSocket amsSocket = SocketList.FirstOrDefault(s => s.IpTarget == ipTarget);
            if (amsSocket != null)
            {
                amsSocket.Subscribers--;
                if (amsSocket.Subscribers <= 0)
                {
                    SocketList.Remove(amsSocket);
                    amsSocket.Dispose();
                }
            }
        }
    }
}
