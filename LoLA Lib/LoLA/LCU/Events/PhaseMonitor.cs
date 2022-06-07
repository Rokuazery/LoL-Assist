using System.Threading.Tasks;
using System.Threading;
using System;

namespace LoLA.LCU.Events
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
        private Phase LastPhase { get; set; }

        public void InitPhaseMonitor()
        {
            Thread MonitorThread = new Thread(PhaseFlowMonitor);
            MonitorThread.IsBackground = true;
            MonitorThread.Start();
        }

        public async void PhaseFlowMonitor()
        {
            while (true)
            {
                if(!IsMonitoring)
                {
                    Thread.Sleep(MonitorDelay * 2);
                    continue;
                }

                var currentPhase = await LCUWrapper.GetGamePhaseAsync();
                if (LastPhase != currentPhase)
                {
                    LastPhase = currentPhase;
                    PhaseChanged?.Invoke(this, new PhaseChangedArgs(currentPhase));
                }
                await Task.Delay(MonitorDelay);
            }
        }
    }
}
