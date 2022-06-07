using System.Threading.Tasks;
using System.Reflection;
using LoLA.Utils.Logger;
using System.Linq;
using LoLA.Utils;
using System;

namespace LoLA.WebAPIs.Metasrc
{
    public class MetasrcClass
    {
        public Key key;
        private const BindingFlags FLAG = BindingFlags.NonPublic | BindingFlags.Instance;
        private const string JSON_URL = "https://onedrive.live.com/download?resid=5E12824F9E63EA74%214965&authkey=AB7i7uPLfWaM4Yc";
        public void Fetch()
        {
            try
            {
                LogService.Log(LogService.Model("Downloading Meta keys...", Global.name, LogType.INFO));

                key = Task.Run(() => WebExt.DlDe<Key>(JSON_URL)).Result;

                if (key == null || string.IsNullOrEmpty(key.Perks)) throw new Exception();

                var names = typeof(Key).GetFields(FLAG).ToList();
                var values = key.GetType().GetFields(FLAG).Select(field => field.GetValue(key)).ToList();

                LogService.Log(LogService.Model("-------MetaKeys--------------------------------", Global.name, LogType.DBUG));
                for (int i = 0; i < names.Count(); i++)
                    LogService.Log(LogService.Model($"{names[i]}: {values[i]}", Global.name, LogType.INFO));
                LogService.Log(LogService.Model("-----------------------------------------------", Global.name, LogType.DBUG));
            }
            catch(Exception) { LogService.Log(LogService.Model("Failed to fetch Meta keys...", Global.name, LogType.EROR)); }
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
