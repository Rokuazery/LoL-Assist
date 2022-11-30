using LoL_Assist_WAPP.Models;
using System.Threading;
using System;

namespace LoL_Assist_WAPP.Threads
{
    public static class AutoMessageThread
    {
        private static bool s_isThreadRunning { get; set; } = true;
        public static string s_Message { get; set; }
        private static Thread s_thread { get; set; }

        public static void InitThread()
        {
         
        }

        private static void autoMessage()
        {
            while(s_isThreadRunning)
            {
                
                Thread.Sleep(ConfigModel.s_Config.MonitoringDelay);
            }
        }

        public static void Start()
        {
            if(!s_thread.IsAlive)
            {
                s_thread = new Thread(autoMessage);
                s_thread.IsBackground = true;

                s_isThreadRunning = true;
                s_thread.Start();
            }
        }

        public static void Stop()
        {
            if (!s_thread.IsAlive)
            {
                s_isThreadRunning = false;
                s_thread.Abort();
            }
        }
    }
}
