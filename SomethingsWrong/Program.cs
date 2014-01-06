using SomethingsWrong.Lib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;

namespace SomethingsWrong
{
    class Program
    {
        static void Main()
        {
            const int httpFailedLightAlarmDurationInSeconds = 10;

            FileInfo controllerFile = GetControllerFile();
            if (!controllerFile.Exists)
            {
                Console.WriteLine("Something's Wrong controller file cannot be found at: " + controllerFile.FullName);
                return;
            }

            IList<MonitorAction> monitorActions = new List<MonitorAction>
            {
                //new TCBuildStatusCheckAction(
                //    new Uri("http://pllod-v-1abb002.pl.abb.com:7080/guestAuth/app/rest/builds/?locator=buildType:Inside_CiAndDeployToAmazon"),
                //    "/builds/build[1]",
                //    "TC: Inside+"),

                //new HttpCheckAction(new Uri("https://insideplus.dev.abb.com"),
                //                    "Copyright 2013 ABB",
                //                    "https://insideplus.dev.abb.com")

                new HttpCheckAction(new Uri("http://localhost:8888"),
                                    "Blah blah!",
                                    "LOCAL http test",
                                    httpFailedLightAlarmDurationInSeconds)
            };

            IList<AlertAction> alertActions = new List<AlertAction>
            {
                new FlashLightAction(controllerFile)
            };

            const int checkIntervalInSeconds = 15;

            var detector = new WrongnessDetector(monitorActions, alertActions);
            while (true)
            {
                try
                {
                    detector.CheckAll();
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                Thread.Sleep(checkIntervalInSeconds * 1000);
            }
        }

        private static FileInfo GetControllerFile()
        {
            string swControllerFilename = "somethingswrong_controller.exe";
            string execPath = Assembly.GetExecutingAssembly().Location;
            string pathToSWController = Path.Combine(execPath, @"..\..\..\..\Lib", swControllerFilename);
            FileInfo fi = new FileInfo(pathToSWController);            
            return fi;
        }

        
    }
}
