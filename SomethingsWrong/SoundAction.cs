
using SomethingsWrong.Lib;
using System;
using System.Diagnostics;
using System.IO;
using System.Media;
using System.Threading;

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
