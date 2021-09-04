using LoLA;
using System;
using LoLA.Data.Objects;
using LoLA.LeagueClient;
using System.Threading.Tasks;
using LoLA.LeagueClient.Objects;

namespace LoL_Assist_WAPP.Model
{
    public class LoLAWrapper
    {
        public static GameFlowMonitor PhaseMonitor = new GameFlowMonitor(Main.leagueClient);
        public static ChampionMonitor ChampMonitor = new ChampionMonitor(Main.leagueClient);

        public static async Task<bool> SetRuneAsync(RuneObj rune, RunePage CurrentRunePage)
        {
            if (CurrentRunePage != null)
            {
                if (CurrentRunePage.name != rune.Name)
                    return await Main.leagueClient.SetRune(DataConverter.RuneBuildToRunePage(rune));

            }
            return false;
        }

        public async static Task ImportSpellsAsync(SpellObj spell, GameMode gameMode)
        {
            await Task.Run(() => {
                if (ConfigM.config.FlashPlacementToRight)
                {
                    if (spell.Spell0 == "SummonerFlash")
                    {
                        var flash = spell.Spell0;
                        spell.Spell0 = spell.Spell1;
                        spell.Spell1 = flash;
                    }
                }

                Main.leagueClient.SetSummonerSpells(
                Dictionaries.SpellIdToKey(spell.Spell0),
                Dictionaries.SpellIdToKey(spell.Spell1), gameMode).Wait();
            });

        }
    }
}
