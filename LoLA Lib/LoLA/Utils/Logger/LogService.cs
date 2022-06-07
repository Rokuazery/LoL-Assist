using System.IO;
using System;

namespace LoLA.Utils.Logger
{
    public static class LogService
    {
        public static readonly string fileName = $"{Global.name}.log";


        public static void Log(LogModel args)
        {
            Append(args);
            if (Global.Config.debug)
            {
                var timeStamp = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
                Console.Write($"[{timeStamp}] ");

                Console.Write(args.type.ToString(), Console.ForegroundColor = GetLogTypeColor(args.type));
                Console.Write($" [{args.source}] ", Console.ForegroundColor = ConsoleColor.DarkGray);
                Console.WriteLine(args.message, Console.ForegroundColor = ConsoleColor.White);

            }
        }

        public static void Append(LogModel args)
        {
            if (Global.Config.logging)
            {
                string logFormat = string.Format("{0} {1} [{2}] >> {3}\n"
                , DateTime.Now, args.type, args.source, args.message);

                if (!File.Exists(fileName)) File.Create(fileName).Dispose();
                File.AppendAllText(fileName, logFormat);
            }
        }

        public static LogModel Model(string msg, string src, LogType type = LogType.UNKN)
        {
            return new LogModel { message = msg, source = src, type = type };
        }

        private static ConsoleColor GetLogTypeColor(LogType type)
        {
            switch (type)
            {
                case LogType.WARN:
                    return ConsoleColor.Yellow;
                case LogType.DBUG:
                    return ConsoleColor.Magenta;
                case LogType.EROR:
                    return ConsoleColor.Red;
                case LogType.INFO:
                    return ConsoleColor.Green;
                default: return ConsoleColor.Cyan;
            }
        }

        public static void Clear()
        {
            File.Delete(fileName);
            File.Create(fileName).Dispose();
        }
    }
}
