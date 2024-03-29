﻿using LoLA.Networking.LCU.Objects;
using LoLA.Networking.LCU.Events;
using Converter = LoLA.Networking.WebWrapper.DataDragon.Data.Converter;
using LoLA.Networking.LCU.Enums;
using System.Threading.Tasks;
using LoLA.Networking.LCU;
using System.Linq;
using LoLA.Data;
using LoLA;
using System;

namespace LoL_Assist_WAPP.Models
{
    public class LoLAWrapper
    {
        public static PhaseMonitor s_PhaseMonitor;
        public static ChampionMonitor s_ChampMonitor;

        public static async Task<bool> SetRuneAsync(Rune rune, RunePage CurrentRunePage, bool forceUpdate = false)
        {
            if (CurrentRunePage == null || (CurrentRunePage.name == rune.Name) && !forceUpdate)
                return false;

            var newRunePage = Converter.RuneToRunePage(rune);
            var currentRunePageId = (await LCUWrapper.GetCurrentRunePageAsync())?.id;

            if (currentRunePageId.HasValue && (await LCUWrapper.DeleteRunePageAsync(currentRunePageId.Value)))
            {
                await LCUWrapper.AddRunePageAsync(newRunePage);
                return true;
            }

            var activeAndDeleteablePage = (await LCUWrapper.GetRunePagesAsync())
                .Where(page => page.isDeletable && page.isActive).FirstOrDefault();

            await LCUWrapper.DeleteRunePageAsync(activeAndDeleteablePage.id);

            newRunePage.order = 0;
            return await LCUWrapper.AddRunePageAsync(newRunePage);
        }

        public async static Task<bool> ImportSpellsAsync(Spell spell, GameMode gameMode)
        {
            var flashId = "SummonerFlash";

            // Swap flash
            if (ConfigModel.s_Config.FlashPlacementToRight && spell.First == flashId ||
                !ConfigModel.s_Config.FlashPlacementToRight && spell.Second == flashId)
                (spell.First, spell.Second) = (spell.Second, spell.First);

            spell.First = DataConverter.SpellIdToSpellKey(spell.First).ToString();
            spell.Second = DataConverter.SpellIdToSpellKey(spell.Second).ToString();

            // if the value not found then the SpellIdToSpellKey should be returning -1 which is invalid value
            if (spell.First == null || spell.Second == null) return false;

            await LCUWrapper.SetSummonerSpellsAsync(spell, gameMode);

            return true;
        }
    }
}
