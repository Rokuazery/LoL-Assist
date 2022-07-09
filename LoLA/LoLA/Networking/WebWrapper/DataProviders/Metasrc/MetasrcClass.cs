using static LoLA.Utils.Logger.LogService;
using LoLA.Networking.Extensions;
using System.Threading.Tasks;
using LoLA.Utils.Logger;
using System.Reflection;
using System.Linq;
using System;

namespace LoLA.Networking.WebWrapper.DataProviders.METAsrc
{
    public static class MetasrcClass
    {
        public static Key s_Key;
        private const BindingFlags FLAG = BindingFlags.NonPublic | BindingFlags.Instance;
        private const string JSON_URL = "https://onedrive.live.com/download?resid=5E12824F9E63EA74%214965&authkey=AB7i7uPLfWaM4Yc";
        public static void Init()
        {
            try
            {
                Log("Downloading Meta keys...", LogType.INFO);

                s_Key = Task.Run(() => WebEx.DlDe<Key>(JSON_URL)).Result;

                if (s_Key == null || string.IsNullOrEmpty(s_Key.Perks)) throw new Exception();

                var names = typeof(Key).GetFields(FLAG).ToList();
                var values = s_Key.GetType().GetFields(FLAG).Select(field => field.GetValue(s_Key)).ToList();

                Log("-------MetaKeys--------------------------------", LogType.DBUG);

                for (int i = 0; i < names.Count(); i++)
                    Log($"{names[i]}: {values[i]}", LibInfo.NAME, LogType.INFO);

                Log("-----------------------------------------------", LogType.DBUG);
            }
            catch (Exception) { Log("Failed to fetch Meta keys...", LogType.EROR); }
        }

        public class Key
        {
            //public int IndexRB { get; set; }
            public string TipRB { get; set; }
            public string SrcRB { get; set; }
            public string RepRB { get; set; }
            public string Perks { get; set; }

            public string Spells { get; set; }
            public int IndexSP { get; set; }
            public string ImgSP { get; set; }
            public string SrcSP { get; set; }
            public int FirstSP { get; set; }
            public int SecondSP { get; set; }
            //public string MISC { get; set; }
            //public string ARAM { get; set; }
            //public string CLASSIC { get; set; }
        }
    }
}
