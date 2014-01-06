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
        const string relativePathToLibDir = @"..\..\..\..\Lib";
        
        const int checkIntervalInSeconds = 15;

        const int buildFailedLightAlarmDurationInSeconds = 300;
        const int httpFailedLightAlarmDurationInSeconds = 20;
        const int standupTimeLightAlarmDurationInSeconds = 20;

        static void Main()
        {
            FileInfo controllerFile = GetControllerFile();
            if (!controllerFile.Exists)
            {
                Console.WriteLine("Something's Wrong controller file cannot be found at: " + controllerFile.FullName);
                return;
            }

            FileInfo buildAlarmSoundFile = GetSoundFilename("builAlarm.wav");
            if (!buildAlarmSoundFile.Exists)
            {
                Console.WriteLine("Build alarm sound file do not exist: " + buildAlarmSoundFile.FullName);
                return;
            }

            FileInfo httpAlarmSoundFile = GetSoundFilename("httpAlarm.wav");
            if (!httpAlarmSoundFile.Exists)
            {
                Console.WriteLine("HTTP alarm sound file do not exist: " + httpAlarmSoundFile.FullName);
                return;
            }

            FileInfo standupSoundFile = GetSoundFilename("standupAlarm.wav");
            if (!standupSoundFile.Exists)
            {
                Console.WriteLine("Standup sound file do not exist: " + standupSoundFile.FullName);
                return;
            }

            IList<MonitorAction> monitorActions = new List<MonitorAction>
            {
                new TCBuildStatusCheckAction(
                                    new Uri("http://pllod-v-1abb002.pl.abb.com:7080/guestAuth/app/rest/builds/?locator=buildType:Inside_CiAndDeployToAmazon"),
                                    "/builds/build[1]",
                                    "TC: Inside+",
                                    buildFailedLightAlarmDurationInSeconds,
                                    buildAlarmSoundFile),

                //new HttpCheckAction(new Uri("https://insideplus.dev.abb.com"),
                //                    "Copyright 2013 ABB",
                //                    "https://insideplus.dev.abb.com")

                new HttpCheckAction(new Uri("http://localhost:8888"),
                                    "Blah blah!",
                                    "LOCAL http test",
                                    httpFailedLightAlarmDurationInSeconds,
                                    httpAlarmSoundFile),

                new TimeCheckAction("Standup time check",
                                    standupTimeLightAlarmDurationInSeconds,
                                    new TimeSpan(0, 36, 0),
                                    standupSoundFile)
            };

            IList<AlertAction> alertActions = new List<AlertAction>
            {
                new FlashLightAction(controllerFile),
                new SoundAction()
            };

            

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

        private static FileInfo GetSoundFilename(string filename)
        {
            string execPath = Assembly.GetExecutingAssembly().Location;
            string path = Path.Combine(execPath, relativePathToLibDir, filename);
            FileInfo fi = new FileInfo(path);
            return fi;
        }

        private static FileInfo GetControllerFile()
        {
            string swControllerFilename = "somethingswrong_controller.exe";
            string execPath = Assembly.GetExecutingAssembly().Location;
            string pathToSWController = Path.Combine(execPath, relativePathToLibDir, swControllerFilename);
            FileInfo fi = new FileInfo(pathToSWController);            
            return fi;
        }

        
    }
}
