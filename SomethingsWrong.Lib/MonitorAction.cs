
namespace SomethingsWrong.Lib
{
    public abstract class MonitorAction
    {
        public string Name {get; private set;}
        public int LightAlarmDurationInSeconds { get; private set; }
        public bool MarkedAsFailing {get; set;}

        public MonitorAction(string name, int lightAlarmDurationInSeconds)
        {
            Name = name;
            LightAlarmDurationInSeconds = lightAlarmDurationInSeconds;
        }

        public abstract bool GetStatus();
        public abstract string GetActionDetails();
    }
}