using System;
using System.IO;
using System.Net;

namespace SomethingsWrong.Lib
{
    public class TimeCheckAction : MonitorAction
    {
        private DateTime? _nextAlarmNoEarlierThan;
        private readonly TimeSpan _time;

        public TimeCheckAction(string name, int lightAlarmDurationInSeconds, TimeSpan time, FileInfo soundFile, bool failAtNetworkException)
            : base(name, lightAlarmDurationInSeconds, soundFile, failAtNetworkException)
        {
            _time = time;
        }

        public override bool GetStatus()
        {   
            //return false == RUN ALARM
            
            if(!PassedNextAlarmTime())
            {
                return true;
            }

            double diffInSec = (DateTime.Now.TimeOfDay - _time).TotalSeconds;

            if ((-30 < diffInSec && diffInSec < 30))
            {
                _nextAlarmNoEarlierThan = DateTime.Now + new TimeSpan(1,0,0);
                return false;
            }
            return true;
        }

        private bool PassedNextAlarmTime()
        {
            if (_nextAlarmNoEarlierThan == null)
            {
                return true;
            }

            return DateTime.Now >= _nextAlarmNoEarlierThan;
        }

        public override string GetActionDetails()
        {
            return string.Format("Checking time for: {0:00}:{1:00}", _time.Hours, _time.Minutes);
        }
    }
}
