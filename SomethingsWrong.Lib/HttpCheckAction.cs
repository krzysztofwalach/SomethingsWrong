using System;
using System.IO;
using System.Net;

namespace SomethingsWrong.Lib
{
    public class HttpCheckAction : MonitorAction
    {
        private readonly Uri _url;
        private readonly string _expectedString;

        public HttpCheckAction(Uri url, string expectedString, string name, int lightAlarmDurationInSeconds, FileInfo soundFile)
            : base(name, lightAlarmDurationInSeconds, soundFile)
        {
            _url = url;
            _expectedString = expectedString;
        }

        public override bool GetStatus()
        {
            using (var wc = new WebClient())
            {
                string results = wc.DownloadString(_url);
                return results.IndexOf(_expectedString, StringComparison.InvariantCultureIgnoreCase) >= 0;
            }
        }

        public override string GetActionDetails()
        {
            return string.Format("Veryfying HTTP status and html at {0}", _url);
        }
    }
}
