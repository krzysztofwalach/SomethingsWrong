using System.Configuration;
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
        private const string RelativePathToLibDir = @"..\..\..\..\Lib";
        private const string SwControllerFilename = "somethingswrong_controller.exe";
        private const int CheckIntervalInSeconds = 15;

        private static readonly int BuildFailedLightAlarmDurationInSeconds = int.Parse(ConfigurationManager.AppSettings["buildFailedLightAlarmDurationInSeconds"]);
        static readonly int HttpFailedLightAlarmDurationInSeconds = int.Parse(ConfigurationManager.AppSettings["httpFailedLightAlarmDurationInSeconds"]);
        static readonly int StandupTimeLightAlarmDurationInSeconds = int.Parse(ConfigurationManager.AppSettings["standupTimeLightAlarmDurationInSeconds"]);

        static void Main()
        {
            FileInfo controllerFile = GetControllerFile();
            if (!controllerFile.Exists)
            {
                Console.WriteLine("Something's Wrong controller file cannot be found at: " + controllerFile.FullName);
                Console.ReadKey();
                return;
            }

            FileInfo buildAlarmSoundFile = GetSoundFilename("buildAlarm.wav");
            if (!buildAlarmSoundFile.Exists)
            {
                Console.WriteLine("Build alarm sound file do not exist: " + buildAlarmSoundFile.FullName);
                Console.ReadKey();
                return;
            }

            FileInfo httpAlarmSoundFile = GetSoundFilename("httpAlarm.wav");
            if (!httpAlarmSoundFile.Exists)
            {
                Console.WriteLine("HTTP alarm sound file do not exist: " + httpAlarmSoundFile.FullName);
                Console.ReadKey();
                return;
            }

            FileInfo standupSoundFile = GetSoundFilename("standupAlarm.wav");
            if (!standupSoundFile.Exists)
            {
                Console.WriteLine("Standup sound file do not exist: " + standupSoundFile.FullName);
                Console.ReadKey();
                return;
            }

            IList<MonitorAction> monitorActions = new List<MonitorAction>();
            if (bool.Parse(ConfigurationManager.AppSettings["EnableTCBuildCheck"]))
            {
                monitorActions.Add(
                    new TCBuildStatusCheckAction(
                        new Uri("http://pllod-v-1abb002.pl.abb.com:7080/guestAuth/app/rest/builds/?locator=buildType:Inside_CiAndDeployToAmazon"),
                        "/builds/build[1]",
                        "TC: Inside+",
                        BuildFailedLightAlarmDurationInSeconds,
                        buildAlarmSoundFile,
                        false));
            }
            if (bool.Parse(ConfigurationManager.AppSettings["EnableDevHTTPCheck"]))
            {
                monitorActions.Add(
                    new HttpCheckAction(new Uri("https://insideplus.dev.abb.com/Monitoring/getheartbeat"),
                        "I am alive!",
                        "https://insideplus.dev.abb.com",
                        HttpFailedLightAlarmDurationInSeconds,
                        httpAlarmSoundFile,
                        true));
                //monitorActions.Add(
                //    new HttpCheckAction(new Uri("https://insideplus.local.abb.com/Monitoring/getheartbeat"),
                //        "I am alive!",
                //        "https://insideplus.local.abb.com",
                //        HttpFailedLightAlarmDurationInSeconds,
                //        httpAlarmSoundFile,
                //        true));
            }
            if (bool.Parse(ConfigurationManager.AppSettings["EnableStandupCheck"]))
            {
                var standupTime = new TimeSpan(
                    int.Parse(ConfigurationManager.AppSettings["StandupHour"]),
                    int.Parse(ConfigurationManager.AppSettings["StandupMinutes"]),
                    0);


                monitorActions.Add(
                    new TimeCheckAction("Standup time check",
                        StandupTimeLightAlarmDurationInSeconds,
                        standupTime,
                        standupSoundFile,
                        false));
            }

            IList<AlertAction> alertActions = new List<AlertAction>();
            if (bool.Parse(ConfigurationManager.AppSettings["EnableFlashLight"]))
            {
                alertActions.Add(new FlashLightAction(controllerFile));
            }
            if (bool.Parse(ConfigurationManager.AppSettings["EnableSounds"]))
            {
                alertActions.Add(new SoundAction());
            }          

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
                Thread.Sleep(CheckIntervalInSeconds * 1000);
            }
        }

        private static FileInfo GetSoundFilename(string filename)
        {
            string execPath = Assembly.GetExecutingAssembly().Location;
            string path = Path.Combine(execPath, RelativePathToLibDir, filename);
            FileInfo fi = new FileInfo(path);
            return fi;
        }

        private static FileInfo GetControllerFile()
        {
            string execPath = Assembly.GetExecutingAssembly().Location;
            string pathToSWController = Path.Combine(execPath, RelativePathToLibDir, SwControllerFilename);
            FileInfo fi = new FileInfo(pathToSWController);            
            return fi;
        }

        
    }
}
