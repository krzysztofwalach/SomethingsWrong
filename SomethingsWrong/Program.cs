using System.Configuration;
using System.Linq;
using NLog;
using SomethingsWrong.Hardware;
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
        private static readonly int CheckIntervalInSeconds = int.Parse(ConfigurationManager.AppSettings["CheckIntervalInSeconds"]);

        private static readonly int BuildFailedLightAlarmDurationInSeconds = int.Parse(ConfigurationManager.AppSettings["buildFailedLightAlarmDurationInSeconds"]);
        static readonly int HttpFailedLightAlarmDurationInSeconds = int.Parse(ConfigurationManager.AppSettings["httpFailedLightAlarmDurationInSeconds"]);
        static readonly int StandupTimeLightAlarmDurationInSeconds = int.Parse(ConfigurationManager.AppSettings["standupTimeLightAlarmDurationInSeconds"]);

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        static void Main()
        {
            FileInfo controllerFile = GetControllerFile();
            if (!controllerFile.Exists)
            {
                Logger.Error("Something's Wrong controller file cannot be found at: " + controllerFile.FullName);
                Console.ReadKey();
                return;
            }

            FileInfo buildAlarmSoundFile = GetSoundFilename("buildAlarm.wav");
            if (!buildAlarmSoundFile.Exists)
            {
                Logger.Error("Build alarm sound file do not exist: " + buildAlarmSoundFile.FullName);
                Console.ReadKey();
                return;
            }

            FileInfo httpAlarmSoundFile = GetSoundFilename("httpAlarm.wav");
            if (!httpAlarmSoundFile.Exists)
            {
                Logger.Error("HTTP alarm sound file do not exist: " + httpAlarmSoundFile.FullName);
                Console.ReadKey();
                return;
            }

            var standupSoundFiles = GetSoundsFilesFromSubDirectory("standupAlarms");
            if (!standupSoundFiles.Any())
            {
                Logger.Error("Didn't find any standup sound files in subdirectory /standupAlarms");
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
                        new List<FileInfo> {buildAlarmSoundFile},
                        false));
            }
            if (bool.Parse(ConfigurationManager.AppSettings["EnableDevHTTPCheck"]))
            {
                monitorActions.Add(
                    new HttpCheckAction(new Uri("https://insideplus2.dev.abb.com/Monitoring/getheartbeat"),
                        "I am alive!",
                        "https://insideplus.dev.abb.com",
                        HttpFailedLightAlarmDurationInSeconds,
                        new List<FileInfo> {httpAlarmSoundFile},
                        true));
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
                        standupSoundFiles,
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
            Console.WriteLine("Loop started");
            while (true)
            {
                
                var date = DateTime.Now;
                if (Calendar.IsHoliday(date))
                {
                    Logger.Debug("It's holidays day, sleeping for 10 minutes...");
                    Thread.Sleep(1000 * 60 * 10);
                    continue;
                }
                if (!Calendar.TimeIsInsideWorkingHours(date))
                {
                    Logger.Debug("The time is outside office hours, sleeping for 10 minutes...");
                    Thread.Sleep(1000 * 60 * 10);
                    continue;
                }

                try
                {
                    detector.CheckAll();
                }
                catch(Exception ex)
                {
                    Logger.Error(ex.Message);
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

        private static IList<FileInfo> GetSoundsFilesFromSubDirectory(string subDirectory)
        {
            string execPath = Assembly.GetExecutingAssembly().Location;
            string path = Path.Combine(execPath, RelativePathToLibDir, subDirectory);
            string[] filePaths = Directory.GetFiles(path, "*.wav");
            var fis = filePaths.Select(fi => new FileInfo(fi)).ToList();
            return fis;
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
