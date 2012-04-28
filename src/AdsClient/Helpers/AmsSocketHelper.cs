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
