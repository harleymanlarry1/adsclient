using Ads.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            StartTest();
            Console.ReadKey();
        }

        private static void StartTest()
        {
            using (AdsClient client = new AdsClient(
                amsNetIdSource: "10.0.0.120.1.1",
                ipTarget: "10.0.0.2",
                amsNetIdTarget: "5.1.204.160.1.1",
                amsPortTarget: 811)) //I'm using the 2nd runtime
            {
                var deviceInfo = client.ReadDeviceInfo();
                Console.WriteLine("Device info: " + deviceInfo.ToString());

                Console.WriteLine();
                Console.WriteLine("Available symbols: ");
                var symbols = client.Special.GetSymbols();
                foreach (var symbol in symbols)
                {
                    Console.WriteLine("  " + symbol.ToString());
                }
                Console.WriteLine();

                uint startTestHandle = client.GetSymhandleByName("MAIN.STARTTEST");
                Console.WriteLine("Handle StartTest: " + startTestHandle.ToString());

                uint testIsRunningHandle = client.GetSymhandleByName("MAIN.TESTISRUNNING");
                Console.WriteLine("Handle TestIsRunning: " + testIsRunningHandle.ToString());

                uint stopTestHandle = client.GetSymhandleByName("MAIN.STOPTEST");
                Console.WriteLine("Handle StopTest: " + stopTestHandle.ToString());

                var testIsRunning = client.Read<bool>(testIsRunningHandle);
                Console.WriteLine("Is test running? " + testIsRunning.ToString());

                client.Write<bool>(startTestHandle, true);
                Console.WriteLine("Starting test");

                testIsRunning = client.Read<bool>(testIsRunningHandle);
                Console.WriteLine("Is test running? " + testIsRunning.ToString());


                //TODO



                client.Write<bool>(stopTestHandle, true);
                Console.WriteLine("Stopping test");

                testIsRunning = client.Read<bool>(testIsRunningHandle);
                Console.WriteLine("Is test running? " + testIsRunning.ToString());
            }
        }
    }
}
