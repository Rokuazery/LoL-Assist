using System.Threading.Tasks;
using System.Threading;
using System;

namespace LoLA.LCU.Events
{
    public class PhaseMonitor
    {
        public class PhaseChangedArgs : EventArgs
        {
            public Phase CurrentPhase { get; set; }

            public PhaseChangedArgs(Phase CurrentPhase)
            {
                this.currentPhase = CurrentPhase;
            }
        }

        public event EventHandler<PhaseChangedArgs> PhaseChanged;
        public bool IsMonitoring { get; set; } = true;
        public int MonitorDelay { get; set; } = 300;
        private Phase lastPhase { get; set; }

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
                if (lastPhase != currentPhase)
                {
                    lastPhase = currentPhase;
                    PhaseChanged?.Invoke(this, new PhaseChangedArgs(currentPhase));
                }
                await Task.Delay(MonitorDelay);
            }
        }
    }
}
