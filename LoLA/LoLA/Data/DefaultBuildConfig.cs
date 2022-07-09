using LoLA.Networking.LCU.Enums;
using LoLA.Data.Enums;


namespace LoLA.Data
{
    public class DefaultBuildConfig
    {
        public string Aram { get; set; }
        public string Classic { get; set; }
        public string PracticeTool { get; set; }
        public string UltBook { get; set; }
        public string TFT { get; set; }
        public string OneForAll { get; set; }
        public string URF { get; set; }
        public string ARURF { get; set; }

        public string GetDefaultConfig(GameMode gameMode)
            => gameMode switch
            {
                GameMode.ARAM => Aram,
                GameMode.CLASSIC => Classic,
                GameMode.PRACTICETOOL => PracticeTool,
                GameMode.ULTBOOK => UltBook,
                GameMode.TFT => TFT,
                GameMode.ONEFORALL => OneForAll,
                GameMode.URF => URF,
                GameMode.ARURF => ARURF,
                _ => null
            };

        public void ResetDefaultConfig(GameMode gameMode)
        {
            switch (gameMode)
            {
                case GameMode.ARAM:
                    Aram = null;
                    break;
                case GameMode.CLASSIC:
                    Classic = null;
                    break;
                case GameMode.PRACTICETOOL:
                    PracticeTool = null;
                    break;
                case GameMode.ULTBOOK:
                    UltBook = null;
                    break;
                case GameMode.TFT:
                    TFT = null;
                    break;
                case GameMode.ONEFORALL:
                    OneForAll = null;
                    break;
                case GameMode.URF:
                    URF = null;
                    break;
                case GameMode.ARURF:
                    ARURF = null;
                    break;
            }
        }

        public void SetDefaultConfig(GameMode gameMode, string config)
        {
            switch (gameMode)
            {
                case GameMode.ARAM:
                    Aram = config;
                    break;
                case GameMode.CLASSIC:
                    Classic = config;
                    break;
                case GameMode.PRACTICETOOL:
                    PracticeTool = config;
                    break;
                case GameMode.ULTBOOK:
                    UltBook = config;
                    break;
                case GameMode.TFT:
                    TFT = config;
                    break;
                case GameMode.ONEFORALL:
                    OneForAll = config;
                    break;
                case GameMode.URF:
                    URF = config;
                    break;
                case GameMode.ARURF:
                    ARURF = config;
                    break;
            }
        }
    }
}
