using System.IO;
using System;

namespace LoLA.Utils.Logger
{
    public static class LogService
    {
        public static readonly string r_FileName = $"{LibInfo.NAME}.log";

        public static void Log(LogModel args)
        {
            Append(args);
            if (GlobalConfig.s_Debug)
            {
                var timeStamp = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
                Console.Write($"[{timeStamp}] ");

                Console.Write(args.Type.ToString(), Console.ForegroundColor = getLogTypeColor(args.Type));
                Console.Write($" [{args.Source}] ", Console.ForegroundColor = ConsoleColor.DarkGray);
                Console.WriteLine(args.Message, Console.ForegroundColor = ConsoleColor.White);
            }
        }

        public static void Log(string message,string source, LogType logType)
        {
            Append(source, message, logType);
            if (GlobalConfig.s_Debug)
            {
                var timeStamp = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
                Console.Write($"[{timeStamp}] ");

                Console.Write(logType.ToString(), Console.ForegroundColor = getLogTypeColor(logType));
                Console.Write($" [{source}] ", Console.ForegroundColor = ConsoleColor.DarkGray);
                Console.WriteLine(message, Console.ForegroundColor = ConsoleColor.White);
            }
        }

        public static void Log(string message, LogType logType)
        {
            Append(LibInfo.NAME, message, logType);
            if (GlobalConfig.s_Debug)
            {
                var timeStamp = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
                Console.Write($"[{timeStamp}] ");

                Console.Write(logType.ToString(), Console.ForegroundColor = getLogTypeColor(logType));
                Console.Write($" [{LibInfo.NAME}] ", Console.ForegroundColor = ConsoleColor.DarkGray);
                Console.WriteLine(message, Console.ForegroundColor = ConsoleColor.White);
            }
        }

        public static void Append(LogModel args)
        {
            if (GlobalConfig.s_Logging)
            {
                string logFormat = $"{DateTime.Now} {args.Type} [{args.Source}] >> {args.Message}\n";
                if (!File.Exists(r_FileName)) File.Create(r_FileName).Dispose();
                File.AppendAllText(r_FileName, logFormat);
            }
        }

        public static void Append(string source, string message, LogType logType)
        {
            if (GlobalConfig.s_Logging)
            {
                string logFormat = $"{DateTime.Now} {logType} [{source}] >> {message}\n";

                if (!File.Exists(r_FileName)) File.Create(r_FileName).Dispose();
                File.AppendAllText(r_FileName, logFormat);
            }
        }

        public static LogModel Model(string msg, string src, LogType type = LogType.UNKN)
        {
            return new LogModel { Message = msg, Source = src, Type = type };
        }

        private static ConsoleColor getLogTypeColor(LogType type)
            => type switch
            {
                LogType.WARN => ConsoleColor.Yellow,
                LogType.DBUG => ConsoleColor.Magenta,
                LogType.EROR => ConsoleColor.Red,
                LogType.INFO => ConsoleColor.Green,
                _ => ConsoleColor.Cyan
            };
        
        public static void Clear()
        {
            File.Delete(r_FileName);
            File.Create(r_FileName).Dispose();
        }
    }
}
