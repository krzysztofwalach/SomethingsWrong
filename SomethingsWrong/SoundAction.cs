
using System;
using System.Collections.Generic;
using System.IO;
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
            FileInfo file = GetRandomSoundFile(monitorAction.SoundFiles);
            using (var player = new SoundPlayer(file.FullName))
            {
                player.Play();
            }
        }

        private FileInfo GetRandomSoundFile(IList<FileInfo> files)
        {
            int index = new Random().Next(0, files.Count);
            return files[index];
        }

        public override void Stop()
        {
        }
    }
}
