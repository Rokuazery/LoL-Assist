using LoLA.Networking.LCU.Enums;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace LoLA.Networking.LCU.Events
{
    public class PhaseMonitor
    {
        public class PhaseChangedArgs : EventArgs
        {
            public Phase currentPhase { get; set; }

            public PhaseChangedArgs(Phase currentPhase)
            {
                this.currentPhase = currentPhase;
            }
        }

        public event EventHandler<PhaseChangedArgs> PhaseChanged;
        public bool IsMonitoring { get; set; } = true;
        public int MonitorDelay { get; set; } = 300;
        private Phase previousPhase { get; set; }

        public PhaseMonitor()
        {
            var monitorThread = new Thread(phaseMonitor);
            monitorThread.IsBackground = true;
            monitorThread.Start();
        }

        public async void phaseMonitor()
        {
            while (true)
            {
                if (!IsMonitoring)
                {
                    Thread.Sleep(MonitorDelay * 2);
                    continue;
                }

                var currentPhase = await LCUWrapper.GetGamePhaseAsync();
                if (previousPhase != currentPhase)
                {
                    previousPhase = currentPhase;
                    PhaseChanged?.Invoke(this, new PhaseChangedArgs(currentPhase));
                }
                await Task.Delay(MonitorDelay);
            }
        }
    }
}
