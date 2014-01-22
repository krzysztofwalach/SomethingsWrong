
using System.Collections.Generic;
using System.IO;
namespace SomethingsWrong.Lib
{
    public abstract class MonitorAction
    {
        public string Name {get; private set;}
        public int LightAlarmDurationInSeconds { get; private set; }
        public bool MarkedAsFailing {get; set;}
        public IList<FileInfo> SoundFiles { get; private set; }
        public bool FailAtNetworkException { get; private set; }

        protected MonitorAction(string name, int lightAlarmDurationInSeconds, IList<FileInfo> soundFiles, bool failAtNetworkException)
        {
            Name = name;
            LightAlarmDurationInSeconds = lightAlarmDurationInSeconds;
            SoundFiles = soundFiles;
            FailAtNetworkException = failAtNetworkException;
        }

        public abstract bool GetStatus();
        public abstract string GetActionDetails();
    }
}