using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using NLog;
using SomethingsWrong.Lib;

namespace SomethingsWrong.Hardware
{
    public class FlashLightAction : AlertAction
    {
        private readonly FileInfo _controllerFile;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();


        public FlashLightAction(FileInfo controllerFile) : base(AlertType.Light)
        {
            _controllerFile = controllerFile;
        }
        
        
        private static Thread alarmThread;

        public override void Start(MonitorAction monitorAction)
        {
            Logger.Info("ENABLING light alarm");

            lock (StatusLockingObj)
            {
                if (!_isRunning)
                {
                    DateTime stopTime = DateTime.Now + new TimeSpan(0, 0, monitorAction.LightAlarmDurationInSeconds);
                    alarmThread = new Thread(() => StopAlarmAtGivenTime(stopTime));
                    alarmThread.Start();
                    _isRunning = true;
                    Exec("on");
                }
            }
        }

        private void StopAlarmAtGivenTime(DateTime stopTime)
        {
            while(true)
            {
                if (DateTime.Now >= stopTime)
                {                    
                    lock (StatusLockingObj)
                    {
                        Logger.Info("Disabling light alarm (timeout)");
                        Exec("off");
                        _isRunning = false;
                        return;
                    }
                }
                Thread.Sleep(1000);               
            }            
        }

        public override void Stop()
        {
            Logger.Info("Disabling light alarm (requested)");

            lock (StatusLockingObj)
            {
                if (_isRunning)
                {
                    _isRunning = false;
                    if(alarmThread != null && alarmThread.IsAlive)
                    {
                        alarmThread.Abort();
                    }
                    alarmThread = null;
                    Exec("off");                    
                }
            }
        }

        private void Exec(string param)
        {
            try
            {
                Process.Start(_controllerFile.FullName, param);
            }
            catch(Exception ex)
            {
                Logger.Error("Failed to execute controller process, details: " + ex);
            }            
        }
    }
}
