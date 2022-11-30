using LoLA.Networking.LCU.Objects;
using LoLA.Networking.LCU.Events;
using System.Collections.Generic;
using Converter = LoLA.Networking.WebWrapper.DataDragon.Data.Converter;
using LoLA.Networking.LCU.Enums;
using System.Threading.Tasks;
using LoLA.Networking.LCU;
using LoLA.Data;
using LoLA;

namespace LoL_Assist_WAPP.Models
{
    public class LoLAWrapper
    {
        public static PhaseMonitor s_PhaseMonitor = new PhaseMonitor();
        public static ChampionMonitor s_ChampMonitor = new ChampionMonitor();

        public static async Task<bool> SetRuneAsync(Rune rune, RunePage CurrentRunePage, bool forceUpdate = false)
        {
            if (CurrentRunePage != null)
            {
                if ((CurrentRunePage.name != rune.Name) || forceUpdate)
                {
                    var runePage = Converter.RuneToRunePage(rune);
                    var currentRunePage = await LCUWrapper.GetCurrentRunePageAsync();
                    if (currentRunePage != null)
                    {
                        ulong selectedId = currentRunePage.id;
                        if (currentRunePage.isDeletable)
                            await LCUWrapper.DeleteRunePageAsync(selectedId);
                    }

                    if (!await LCUWrapper.AddRunePageAsync(runePage))
                    {
                        List<RunePage> pages = await LCUWrapper.GetRunePagesAsync();
                        foreach (RunePage page in pages)
                        {
                            if (page.isDeletable && page.isActive)
                            {
                                runePage.order = 0;
                                await LCUWrapper.DeleteRunePageAsync(page.id);
                                return await LCUWrapper.AddRunePageAsync(runePage);
                            }
                        }
                    }
                }
            }
            return false;
        }

        public async static Task ImportSpellsAsync(Spell spell, GameMode gameMode)
        {
            await Task.Run(() => {
                var flashId = "SummonerFlash";
                if (ConfigModel.s_Config.FlashPlacementToRight)
                {
                    if (spell.First.Equals(flashId))
                    {
                        var flash = spell.First;
                        spell.First = spell.Second;
                        spell.Second = flash;
                    }
                }
                else
                {
                    if (spell.Second.Equals(flashId))
                    {
                        var flash = spell.Second;
                        spell.Second = spell.First;
                        spell.First = flash;
                    }
                }
                spell.First = DataConverter.SpellIdToSpellKey(spell.First).ToString();
                spell.Second = DataConverter.SpellIdToSpellKey(spell.Second).ToString();
                LCUWrapper.SetSummonerSpellsAsync(spell, gameMode).Wait();
            });
        }
    }
}
