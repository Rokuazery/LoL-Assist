using LoLA;
using System;
using LoLA.Data.Objects;
using System.Threading.Tasks;
using LoLA.LeagueClient.Objects;

namespace LoL_Assist_WAPP.Model
{
    public class LoLAWrapper
    {
        public static async Task<bool> SetRuneAsync(RuneObj rune, RunePage CurrentRunePage, bool IsUsingV2)
        {
            if (CurrentRunePage != null)
            {
                if (CurrentRunePage.name != rune.Name)
                {
                    if (IsUsingV2)
                        return await Main.leagueClient.SetRune(DataConverter.RuneBuildToRunePageV2(rune));
                    else
                        return await Main.leagueClient.SetRune(DataConverter.RuneBuildToRunePage(rune));
                }

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
