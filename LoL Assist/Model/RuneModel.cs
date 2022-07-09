using System.Collections.ObjectModel;
using System.Collections.Generic;
using LoL_Assist_WAPP.Utils;
using System;

namespace LoL_Assist_WAPP.Model
{
    public static class RuneModel
    {
        private readonly static string[,] r_domination = new string[3,4];
        private readonly static string[,] r_precision = new string[3,3];
        private readonly static string[,] r_sorcery = new string[3,3];
        private readonly static string[,] r_resolve = new string[3,3];
        private readonly static string[,] r_inspiration = new string[3,3];

        public readonly static Dictionary<string, string[,]> r_SecondPath = new Dictionary<string, string[,]>();

        public static void Init()
        {
            #region Domination
            r_domination[0, 0] = "Cheap Shot";
            r_domination[0, 1] = "Taste of Blood";
            r_domination[0, 2] = "Sudden Impact";
         
            r_domination[1, 0] = "Zombie Ward";
            r_domination[1, 1] = "Ghost Poro";
            r_domination[1, 2] = "Eyeball Collection";

            r_domination[2, 0] = "Treasure Hunter";
            r_domination[2, 1] = "Ingenious Hunter";
            r_domination[2, 2] = "Relentless Hunter";
            r_domination[2, 3] = "Ultimate Hunter";
            #endregion

            #region Precision
            r_precision[0, 0] = "Overheal";
            r_precision[0, 1] = "Triumph";
            r_precision[0, 2] = "Presence of Mind";

            r_precision[1, 0] = "Legend: Alacrity";
            r_precision[1, 1] = "Legend: Tenacity";
            r_precision[1, 2] = "Legend: Bloodline";

            r_precision[2, 0] = "Coup de Grace";
            r_precision[2, 1] = "Cut Down";
            r_precision[2, 2] = "Last Stand";
            #endregion

            #region Sorcery
            r_sorcery[0, 0] = "Nullifying Orb";
            r_sorcery[0, 1] = "Manaflow Band";
            r_sorcery[0, 2] = "Nimbus Cloak";
            
            r_sorcery[1, 0] = "Transcendence";
            r_sorcery[1, 1] = "Celerity";
            r_sorcery[1, 2] = "Absolute Focus";
            
            r_sorcery[2, 0] = "Scorch";
            r_sorcery[2, 1] = "Waterwalking";
            r_sorcery[2, 2] = "Gathering Storm";
            #endregion

            #region Resolve
            r_resolve[0, 0] = "Demolish";
            r_resolve[0, 1] = "Font of Life";
            r_resolve[0, 2] = "Shield Bash";

            r_resolve[1, 0] = "Conditioning";
            r_resolve[1, 1] = "Second Wind";
            r_resolve[1, 2] = "Bone Plating";

            r_resolve[2, 0] = "Overgrowth";
            r_resolve[2, 1] = "Revitalize";
            r_resolve[2, 2] = "Unflinching";
            #endregion

            #region Inspiration
            r_inspiration[0, 0] = "Hextech Flashtraption";
            r_inspiration[0, 1] = "Magical Footwear";
            r_inspiration[0, 2] = "Perfect Timing";
      
            r_inspiration[1, 0] = "Future's Market";
            r_inspiration[1, 1] = "Minion Dematerializer";
            r_inspiration[1, 2] = "Biscuit Delivery";
          
            r_inspiration[2, 0] = "Cosmic Insight";
            r_inspiration[2, 1] = "Approach Velocity";
            r_inspiration[2, 2] = "Time Warp Tonic";
            #endregion

            r_SecondPath.Add("Domination", r_domination);
            r_SecondPath.Add("Sorcery", r_sorcery);
            r_SecondPath.Add("Precision", r_precision);
            r_SecondPath.Add("Inspiration", r_inspiration);
            r_SecondPath.Add("Resolve", r_resolve);
        }

        public static ObservableCollection<ItemImageModel> Shards(int row,bool isGrayscaleImage)
        {
            var shards = new ObservableCollection<ItemImageModel>(); 
            var grayScale = isGrayscaleImage ? "g_" : string.Empty;
            switch (row)
            {
                case 1:
                    shards.Add(Helper.ItemImage("5.4 bonus AD or 9 AP (Adaptive)", Helper.ImageSrc($"{grayScale}diamond")));
                    shards.Add(Helper.ItemImage("10% bonus attack speed", Helper.ImageSrc($"{grayScale}axe")));
                    shards.Add(Helper.ItemImage("8 ability haste", Helper.ImageSrc($"{grayScale}time")));
                    break;
                case 2:
                    shards.Add(Helper.ItemImage("5.4 bonus AD or 9 AP (Adaptive)", Helper.ImageSrc($"{grayScale}diamond")));
                    shards.Add(Helper.ItemImage("6 bonus armor", Helper.ImageSrc($"{grayScale}shield")));
                    shards.Add(Helper.ItemImage("8 bonus magic resistance", Helper.ImageSrc($"{grayScale}circle")));
                    break;
                case 3:
                    shards.Add(Helper.ItemImage("15 − 90 (based on level) bonus health", Helper.ImageSrc($"{grayScale}heart")));
                    shards.Add(Helper.ItemImage("6 bonus armor", Helper.ImageSrc($"{grayScale}shield")));
                    shards.Add(Helper.ItemImage("8 bonus magic resistance", Helper.ImageSrc($"{grayScale}circle")));
                    break;
            }
            return shards;
        }
    }
}
