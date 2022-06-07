using System.Threading.Tasks;
using System.Threading;
using System;

namespace LoLA.LCU.Events
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

        public void InitChampionMonitor()
        {
            Thread MonitorThread = new Thread(ChampionMonitorA)
            {
                IsBackground = true
            };
            MonitorThread.Start();
        }

        public bool IsMonitoring { get; set; } = true;
        public int MonitorDelay { get; set; } = 300;
        private string LastChampion { get; set; }

        public async void ChampionMonitorA()
        {
            while (true)
            {
                if (!IsMonitoring)
                {
                    Thread.Sleep(MonitorDelay * 2);
                    continue;
                }

                var Phase = await LCUWrapper.GetGamePhaseAsync();
                if (Phase != Phase.InProgress)
                {
                    var CurrentChampion = await LCUWrapper.GetCurrentChampionAsyncV2();

                    //Console.WriteLine("Champion: " + CurrentChampion);
                    if (LastChampion != CurrentChampion)
                    {
                        LastChampion = CurrentChampion;
                        ChampionChanged?.Invoke(this, new ChampionChangedArgs(CurrentChampion));
                    }
                }
                await Task.Delay(MonitorDelay);
            }
        }
    }
}
