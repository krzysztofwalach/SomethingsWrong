
using System.Diagnostics;

namespace SomethingsWrong
{
    public class FlashLightRunner
    {
        private const string FileName = "ControlerFileName.exe";

        private static bool _isRunning;
        private static object _lockingObj = new object();

        public static void Start()
        {
            lock (_lockingObj)
            {
                if (!_isRunning)
                {
                    _isRunning = true;
                    Exec("on");
                }
            }
        }

        public static void Stop()
        {
            lock (_lockingObj)
            {
                if (_isRunning)
                {
                    _isRunning = false;
                    Exec("off");
                }
            }
        }

        public static bool IsRunning()
        {
            lock (_lockingObj)
            {
                return _isRunning;
            }
        }

        public static void Exec(string param)
        {
            Process.Start(FileName, param);
        }

    }
}
