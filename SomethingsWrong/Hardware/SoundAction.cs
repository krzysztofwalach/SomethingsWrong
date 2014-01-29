using System;
using System.Collections.Generic;
using System.IO;
using System.Media;
using NLog;
using SomethingsWrong.Lib;

namespace SomethingsWrong.Hardware
{
    public class SoundAction : AlertAction
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public SoundAction()
            : base(AlertType.Sound)
        {
        }       
        
        public override void Start(MonitorAction monitorAction)
        {
            Logger.Info("ENABLING sound alarm: " + ListFiles(monitorAction.SoundFiles));

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

        private string ListFiles(IList<FileInfo> files)
        {
            string r = "";
            foreach (var fileInfo in files)
            {
                r += fileInfo.Name + "; ";
            }
            return r;
        }

        public override void Stop()
        {
        }
    }
}
