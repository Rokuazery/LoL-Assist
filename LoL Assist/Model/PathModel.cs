using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoL_Assist_WAPP.Model
{
    public static class PathModel
    {
        public static string[,] Domination = new string[3,4];
        public static string[,] Precision = new string[3,3];
        public static string[,] Sorcery = new string[3,3];
        public static string[,] Resolve = new string[3,3];
        public static string[,] Inspiration = new string[3,3];

        public static void Init()
        {
            #region Domination
            Domination[0, 0] = "Cheap Shot";
            Domination[0, 1] = "Taste of Blood";
            Domination[0, 2] = "Sudden Impact";

            Domination[1, 0] = "Zombie Ward";
            Domination[1, 1] = "Ghost Poro";
            Domination[1, 2] = "Eyeball Collection";

            Domination[2, 0] = "Ravenous Hunter";
            Domination[2, 1] = "Ingenious Hunter";
            Domination[2, 2] = "Relentless Hunter";
            Domination[2, 3] = "Ultimate Hunter";
            #endregion

            #region Precision
            Precision[0, 0] = "Overheal";
            Precision[0, 1] = "Triumph";
            Precision[0, 2] = "Presence of Mind";

            Precision[1, 0] = "Legend: Alacrity";
            Precision[1, 1] = "Legend: Tenacity";
            Precision[1, 2] = "Legend: Bloodline";

            Precision[2, 0] = "Coup De Grace";
            Precision[2, 1] = "Cut Down";
            Precision[2, 2] = "Last Stand";
            #endregion

            #region Sorcery
            Sorcery[0, 0] = "Nullifying Orb";
            Sorcery[0, 1] = "Manaflow Band";
            Sorcery[0, 2] = "Nimbus Cloak";

            Sorcery[1, 0] = "Transcendence";
            Sorcery[1, 1] = "Celerity";
            Sorcery[1, 2] = "Absolute Focus";

            Sorcery[2, 0] = "Scorch";
            Sorcery[2, 1] = "Waterwalking";
            Sorcery[2, 2] = "Gathering Storm";
            #endregion

            #region Resolve
            Resolve[0, 0] = "Demolish";
            Resolve[0, 1] = "Font of Life";
            Resolve[0, 2] = "Shield Bash";

            Resolve[1, 0] = "Conditioning";
            Resolve[1, 1] = "Second Wind";
            Resolve[1, 2] = "Bone Plating";

            Resolve[2, 0] = "Overgrowth";
            Resolve[2, 1] = "Revitalize";
            Resolve[2, 2] = "Unflinching";
            #endregion

            #region Inspiration
            Inspiration[0, 0] = "Hextech Flashtraption";
            Inspiration[0, 1] = "Magical Footwear";
            Inspiration[0, 2] = "Perfect Timing";

            Inspiration[1, 0] = "Future's Market";
            Inspiration[1, 1] = "Minion Dematerializer";
            Inspiration[1, 2] = "Biscuit Delivery";

            Inspiration[2, 0] = "Cosmic Insight";
            Inspiration[2, 1] = "Approach Velocity";
            Inspiration[2, 2] = "Time Warp Tonic";
            #endregion
        }
    }
}
