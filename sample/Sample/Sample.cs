using Ads.Client;
using Ads.Client.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample
{
    public class Sample
    {
        private IAdsConnectionSettings settings;

        public Sample(IAdsConnectionSettings settings)
        {
            this.settings = settings;
        }

        public void RunTest()
        {
            using (AdsClient client = new AdsClient(settings))
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
