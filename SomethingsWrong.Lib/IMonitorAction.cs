
namespace SomethingsWrong.Lib
{
    public abstract class MonitorAction
    {
        private readonly string _name;

        public MonitorAction(string name)
        {
            _name = name;
        }

        public string GetStepName()
        {
            return _name;
        }

        public abstract bool GetStatus();
        public abstract string GetActionDetails();
    }
}