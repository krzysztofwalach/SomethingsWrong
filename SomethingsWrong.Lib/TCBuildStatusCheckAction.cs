using System;
using System.IO;
using System.Net;
using System.Xml;

namespace SomethingsWrong.Lib
{
    public class TCBuildStatusCheckAction : MonitorAction
    {
        private readonly Uri _uriToCheck;
        private readonly string _xmlNodeToVerify;

        public TCBuildStatusCheckAction(Uri uriToCheck, string xmlNodeToVerify, string name, int lightAlarmDurationInSeconds, FileInfo soundFile)
            : base(name, lightAlarmDurationInSeconds, soundFile)
        {
            _uriToCheck = uriToCheck;
            _xmlNodeToVerify = xmlNodeToVerify;
        }

        public override bool GetStatus()
        {
            using (var wc = new WebClient())
            {
                var results = wc.DownloadString(_uriToCheck);
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(results);
                bool wasLastBuildSuccessfull = xmlDoc.SelectSingleNode(_xmlNodeToVerify).Attributes["status"].Value == "SUCCESS";
                return wasLastBuildSuccessfull;
            }
        }

        public override string GetActionDetails()
        {
            return string.Format("Checking build status at {0}", _uriToCheck.AbsolutePath);
        }
    }
}
