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

using System.Net;
using System.Threading.Tasks;
using Ads.Client.Common;

namespace Ads.Client
{
    public delegate void AmsSocketResponseDelegate(object sender, AmsSocketResponseArgs e); 

    public interface IAmsSocket
    {
        bool Connected { get; }
        bool? ConnectedAsync { get; }
        IPEndPoint LocalEndPoint { get; set; }
        bool Verbose { get; set; }
        event AmsSocketResponseDelegate OnReadCallBack;


        #if !SILVERLIGHT
        void ConnectAndListen();
        void Send(byte[] message);
        #endif

        #if !NO_ASYNC
        Task ConnectAndListenAsync();
        Task<bool> SendAsync(byte[] message);
        #endif
    }
}
