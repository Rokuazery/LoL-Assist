using LoLA.Networking.WebWrapper.DataDragon.Data;
using LoLA.Networking.LCU.Objects;
using LoLA.Networking.LCU.Events;
using System.Collections.Generic;
using LoLA.Networking.LCU.Enums;
using System.Threading.Tasks;
using LoLA.Networking.LCU;
using LoLA.Data;
using LoLA;
using Converter = LoLA.Networking.WebWrapper.DataDragon.Data.Converter;

namespace LoL_Assist_WAPP.Model
{
    public class LoLAWrapper
    {
        public static PhaseMonitor phaseMonitor = new PhaseMonitor();
        public static ChampionMonitor champMonitor = new ChampionMonitor();

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
