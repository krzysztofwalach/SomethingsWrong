
using SomethingsWrong.Lib;
using System.Media;

namespace SomethingsWrong
{
    public class SoundAction : AlertAction
    {
        public SoundAction()
            : base(AlertType.Sound)
        {
        }       
        
        public override void Start(MonitorAction monitorAction)
        {
            using(SoundPlayer player = new SoundPlayer(monitorAction.SoundFile.FullName))
            {
                player.Play();
            }
        }

        public override void Stop()
        {
        }
    }
}
