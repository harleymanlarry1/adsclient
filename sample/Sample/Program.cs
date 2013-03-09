using Ads.Client.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var settings = new AdsConnectionSettings();

            //Destination IP
            settings.IpTarget = "10.0.0.2";
            if (args.Count() > 0) settings.IpTarget = GetIpFromHostname(args[0]);

            //Source AmsNetId
            settings.AmsNetIdSource = GetMyIp() + ".1.1";
            if (args.Count() > 1) settings.AmsNetIdSource = args[1];

            //Target AmsNetId
            settings.AmsNetIdTarget = "5.1.204.160.1.1";
            if (args.Count() > 2) settings.AmsNetIdTarget = args[2];

            //Target Ams Port
            settings.AmsPortTarget = 811; //I'm using the 2nd runtime here

            var sample = new Sample(settings);
            sample.RunTest();
            Console.ReadKey();
        }

        private static string GetMyIp()
        {
            string ip = "";
            string sHostName = Dns.GetHostName();
            IPAddress[] ipAddress = Dns.GetHostAddresses(sHostName);
            for (int i = 0; i < ipAddress.Length; i++)
            {
                if (ipAddress[i].AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    ip = ipAddress[i].ToString();
            }
            return ip;
        }

        private static string GetIpFromHostname(string hostname)
        {
            Console.Write("Getting ip... ");
            var ip = "";
            IPAddress[] addresslist = Dns.GetHostAddresses(hostname);

            foreach (IPAddress theaddress in addresslist)
            {
                ip = theaddress.ToString();
                Console.WriteLine(ip);
            };

            return ip;
        }

    }
}
