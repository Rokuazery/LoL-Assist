using System.Threading.Tasks;
using LoLA.LCU.Objects;
using LoLA.LCU.Events;
using LoLA.Objects;
using System.IO;
using LoLA.LCU;
using System;
using LoLA;

namespace LoL_Assist_WAPP.Model
{
    public class LoLAWrapper
    {
        public static PhaseMonitor phaseMonitor = new PhaseMonitor();
        public static ChampionMonitor champMonitor = new ChampionMonitor();

        public static async Task<bool> SetRuneAsync(RuneObj rune, RunePage CurrentRunePage)
        {
            if (CurrentRunePage != null)
            {
                if (CurrentRunePage.name != rune.Name)
                    return await LCUWrapper.SetRune(DataConverter.RuneBuildToRunePage(rune));
            }
            return false;
        }

        public static string GetLocalBuildName(string championId, GameMode gameMode)
        {
            var defaultBuildConfig = new DefaultBuildConfig();
            if (File.Exists(Utils.defConfigPath(championId)))
                defaultBuildConfig = Utils.getDefaultBuildConfig(championId);

            var buildName = defaultBuildConfig.getDefaultConfig(gameMode);

            var customBuildPath = Main.localBuild.BuildsFolder(championId, gameMode);
            var fullPath = $"{customBuildPath}\\{defaultBuildConfig.getDefaultConfig(gameMode)}";

            if (!File.Exists(fullPath))
                defaultBuildConfig.resetDefaultConfig(gameMode);

            Utils.writeDefaultBuildConfig(championId, defaultBuildConfig);
            return buildName;
        }

        public async static Task ImportSpellsAsync(SpellObj spell, GameMode gameMode)
        {
            await Task.Run(() => {
                var flashId = "SummonerFlash";
                if (ConfigModel.config.FlashPlacementToRight)
                {
                    if (spell.Spell0.Equals(flashId))
                    {
                        var flash = spell.Spell0;
                        spell.Spell0 = spell.Spell1;
                        spell.Spell1 = flash;
                    }
                }
                else
                {
                    if (spell.Spell1.Equals(flashId))
                    {
                        var flash = spell.Spell1;
                        spell.Spell1 = spell.Spell0;
                        spell.Spell0 = flash;
                    }
                }
                spell.Spell0 = Dictionaries.SpellIdToSpellKey[spell.Spell0].ToString();
                spell.Spell1 = Dictionaries.SpellIdToSpellKey[spell.Spell1].ToString();
                LCUWrapper.SetSummonerSpells(spell, gameMode).Wait();
            });
        }
    }
}
