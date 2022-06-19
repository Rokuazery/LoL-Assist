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
            public string ChampionName { get; set; }

            public ChampionChangedArgs(string championName)
            {
                this.ChampionName = championName;
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
        private string _lastChampion { get; set; }

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
                    if (_lastChampion != CurrentChampion)
                    {
                        _lastChampion = CurrentChampion;
                        ChampionChanged?.Invoke(this, new ChampionChangedArgs(CurrentChampion));
                    }
                }
                await Task.Delay(MonitorDelay);
            }
        }
    }
}
