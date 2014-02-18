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
        
        
        private static Thread _alarmThread;

        public override void Start(MonitorAction monitorAction)
        {
            Logger.Info("ENABLING light alarm");

            lock (StatusLockingObj)
            {
                if (!_isRunning)
                {
                    DateTime stopTime = DateTime.Now + new TimeSpan(0, 0, monitorAction.LightAlarmDurationInSeconds);
                    _alarmThread = new Thread(() => StopAlarmAtGivenTime(stopTime));
                    _alarmThread.Start();
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
                    if(_alarmThread != null && _alarmThread.IsAlive)
                    {
                        _alarmThread.Abort();
                    }
                    _alarmThread = null;
                    Exec("off");                    
                }
            }
        }

        private void Exec(string param)
        {
            try
            {
                var proc = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = _controllerFile.FullName,
                        Arguments = param,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };
                proc.Start();
                proc.WaitForExit(1000 * 60);

                int code = proc.ExitCode;
                //Logger.Info("Controller process exited with code: " + code);
                Logger.Info("Controller process exited with code: " + code);

                if (code != 0)
                {
                    Logger.Error("Controller process exited with non zero code!");
                }
            }
            catch(Exception ex)
            {
                Logger.Error("Failed to execute controller process, details: " + ex);
            }            
        }
    }
}
