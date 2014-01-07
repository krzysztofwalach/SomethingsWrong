
using System.IO;
namespace SomethingsWrong.Lib
{
    public abstract class MonitorAction
    {
        public string Name {get; private set;}
        public int LightAlarmDurationInSeconds { get; private set; }
        public bool MarkedAsFailing {get; set;}
        public FileInfo SoundFile { get; private set; }
        public bool FailAtNetworkException { get; private set; }

        public MonitorAction(string name, int lightAlarmDurationInSeconds, FileInfo soundFile, bool failAtNetworkException)
        {
            Name = name;
            LightAlarmDurationInSeconds = lightAlarmDurationInSeconds;
            SoundFile = soundFile;
            FailAtNetworkException = failAtNetworkException;
        }

        public abstract bool GetStatus();
        public abstract string GetActionDetails();
    }
}