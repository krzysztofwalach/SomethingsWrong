using SomethingsWrong.Lib;
using System;
using System.Collections.Generic;
using System.Threading;

namespace SomethingsWrong
{
    class Program
    {
        static void Main()
        {
            IList<MonitorAction> monitorActions = new List<MonitorAction>
            {
                new TCBuildStatusCheckAction(
                    new Uri("http://pllod-v-1abb002.pl.abb.com:7080/guestAuth/app/rest/builds/?locator=buildType:Inside_CiAndDeployToAmazon"),
                    "/builds/build[1]",
                    "TC: Inside+"),

                new HttpCheckAction(new Uri("https://insideplus.dev.abb.com"),
                                    "Copyright 2013 ABB",
                                    "https://insideplus.dev.abb.com")
            };

            const int checkIntervalInSeconds = 15;

            var detector = new WrongnessDetector(monitorActions);
            while (true)
            {
                detector.CheckAll();
                Thread.Sleep(checkIntervalInSeconds * 1000);
            }
        }

        
    }
}
