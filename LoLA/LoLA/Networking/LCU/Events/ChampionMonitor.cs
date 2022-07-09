using LoLA.Networking.LCU.Enums;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace LoLA.Networking.LCU.Events
{
    public class ChampionMonitor
    {
        public event EventHandler<ChampionChangedArgs> ChampionChanged;

        public class ChampionChangedArgs : EventArgs
        {
            public string championName { get; set; }

            public ChampionChangedArgs(string championName)
            {
                this.championName = championName;
            }
        }

        public ChampionMonitor()
        {
            Thread MonitorThread = new Thread(championMonitor);
            MonitorThread.IsBackground = true;
            MonitorThread.Start();
        }

        public bool IsMonitoring { get; set; } = true;
        public int MonitorDelay { get; set; } = 300;
        private string LastChampion { get; set; }

        private async void championMonitor()
        {
            while (true)
            {
                if (!IsMonitoring)
                {
                    Thread.Sleep(MonitorDelay * 2);
                    continue;
                }

                var phase = await LCUWrapper.GetGamePhaseAsync();
                if (phase != Phase.InProgress)
                {
                    var currentChampion = await LCUWrapper.GetCurrentChampionAsyncV2();

                    if (LastChampion != currentChampion)
                    {
                        LastChampion = currentChampion;
                        ChampionChanged?.Invoke(this, new ChampionChangedArgs(currentChampion));
                    }
                }
                await Task.Delay(MonitorDelay);
            }
        }
    }
}
