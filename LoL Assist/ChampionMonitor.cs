using LoLA;
using System;
using System.Threading;
using LoL_Assist_WAPP.Model;

namespace LoL_Assist_WAPP
{
    public class ChampionMonitor
    {
        public LoLA.LeagueClient.RequestWrapper wrapper;
        public EventHandler<ChampionChangedArgs> ChampionChanged;

        public class ChampionChangedArgs : EventArgs
        {
            public string Name { get; set; }

            public ChampionChangedArgs(string currentChampionName)
            {
                Name = currentChampionName;
            }
        }

        public ChampionMonitor(LoLA.LeagueClient.RequestWrapper requestWrapper)
        {
            wrapper = requestWrapper;
        }

        public bool IsMonitoring;
        private string LastChampion;
        private Thread MonitorThread;
        public string CurrentChampion;
        public Phase CurrentPhase { get; set; }

        public void InitMonitor()
        {
            IsMonitoring = true;
            MonitorThread = new Thread(ChampionMonitorA);
            MonitorThread.Start();
        }

        public async void ChampionMonitorA()
        {
            while (IsMonitoring)
            {
                if (CurrentPhase != Phase.InProgress)
                {
                    CurrentChampion = await wrapper?.GetCurrentChampionAsyncV2();
                    if (LastChampion != CurrentChampion)
                    {
                        LastChampion = CurrentChampion;
                        ChampionChanged.Invoke(this, new ChampionChangedArgs(CurrentChampion));
                    }
                }
                Thread.Sleep(ConfigM.config.MonitoringDelay);
            }
        }
    }
}
