using LoLA.Networking.LCU.Events;
using LoLA.Networking.LCU.Enums;
using LoL_Assist_WAPP.Models;
using LoLA.Networking.LCU;
using System;

namespace LoL_Assist_WAPP.Features
{
    public static class AutoMessage
    {
        public static void Init()
            => LoLAWrapper.s_PhaseMonitor.PhaseChanged += phase_Changed;

        private static async void phase_Changed(object sender, PhaseMonitor.PhaseChangedArgs e)
        {
            if (ConfigModel.s_Config.AutoMessage)
            {
                if (e.currentPhase == Phase.ChampSelect)
                {
                    var type = e.currentPhase.ToString();
                    var convo = await LCUWrapper.GetConversationAsync();

                    await LCUWrapper.SendMessageAsync(convo, ConfigModel.s_Config.Message, type);

                    if (ConfigModel.s_Config.ClearMessageAfterSent)
                        ConfigModel.s_Config.Message = string.Empty;
                }
            }
        }
    }
}
