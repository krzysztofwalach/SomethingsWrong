using NLog;
using System;
using System.Configuration;

namespace SomethingsWrong
{
    public class MultiLogger
    {
        private static readonly string ConsoleLoggingLevel = ConfigurationManager.AppSettings["ConsoleLoggingLevel"];
        private static readonly Logger Logger = LogManager.GetLogger("MyClassName");

        public static void Info(string message)
        {
            WriteToConsole(message);
            Logger.Info(message);
        }

        public static void Error(string message)
        {
            WriteToConsole(message, ConsoleColor.Red);
            Logger.Error(message);
        }

        public static void MonitoredAppFail(string message)
        {
            WriteToConsole(message, ConsoleColor.Yellow);
            Logger.Info(message);
        }

        public static void Debug(string message)
        {
            if (ConsoleLoggingLevel == "debug")
            {
                WriteToConsole(message, ConsoleColor.DarkGray);
            }
            Logger.Debug(message);
        }

        private static void WriteToConsole(string message, ConsoleColor? color = null)
        {
            Console.Write(DateTime.Now.ToUniversalTime() + " ");
            if (color != null)
            {
                Console.ForegroundColor = color.Value;
            }
            Console.WriteLine(message);
            if (color != null)
            {
                Console.ResetColor();
            }
        }
    }
}
