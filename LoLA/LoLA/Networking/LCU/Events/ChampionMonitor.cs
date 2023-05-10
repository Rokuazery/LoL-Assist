using LoLA.Networking.LCU.Enums;
using System.Threading.Tasks;
using System.Threading;
using System;
using LoLA.Utils.Logger;
using LoLA.Networking.WebWrapper.DataDragon.Data;

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
        public ulong SummonerId { get; set; }

        private async void championMonitor()
        {
            while (true)
            {
                if (!IsMonitoring || SummonerId == 0)
                {
                    Thread.Sleep(MonitorDelay * 2);
                    continue;
                }

                var phase = await LCUWrapper.GetGamePhaseAsync();
                if (phase == Phase.ChampSelect)
                {
             
                    var currentChampion = (await LCUWrapper.GetCurrentChampionIdAsync(SummonerId)).ToString();
                    currentChampion = Converter.ChampionKeyToName(currentChampion);

                    // Fallback if the mode doesn't support /lol-champ-select-legacy/v1/session
                    if (string.IsNullOrEmpty(currentChampion))
                        currentChampion = await LCUWrapper.GetCurrentChampionAsyncV2();

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
