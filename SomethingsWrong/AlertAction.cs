using SomethingsWrong.Lib;
using System;
namespace SomethingsWrong
{
    public abstract class AlertAction
    {
        public AlertAction(AlertType type)
        {
            AlertType = type;
        }

        public AlertType AlertType { get; private set; }
        protected static bool _isRunning;
        protected static object StatusLockingObj = new object();

        public bool IsRunning()
        {
            lock (StatusLockingObj)
            {
                return _isRunning;
            }
        }

        public abstract void Start(MonitorAction monitorAction);
        public abstract void Stop();
    }
}
