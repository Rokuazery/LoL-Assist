using static LoL_Assist_WAPP.Model.ConfigModel;
using LoLA.LCU;

namespace LoL_Assist_WAPP.Model
{
    public class DefaultBuildConfig
    {
        public string ARAM_Default { get; set; } = DefaultSource;
        public string CLASSIC_Default { get; set; } = DefaultSource;
        public string PRACTICETOOL_Default { get; set; } = DefaultSource;
        public string ULTBOOK_Default { get; set; } = DefaultSource;
        public string TFT_Default { get; set; } = DefaultSource;
        public string ONEFORALL_Default { get; set; } = DefaultSource;
        public string URF_Default { get; set; } = DefaultSource;
        public string ARURF_Default { get; set; } = DefaultSource;

        public string getDefaultConfig(GameMode gm)
        {
            switch(gm)
            {
                case GameMode.ARAM:
                    return ARAM_Default;
                case GameMode.CLASSIC:
                    return CLASSIC_Default;
                case GameMode.PRACTICETOOL:
                    return PRACTICETOOL_Default;
                case GameMode.ULTBOOK:
                    return ULTBOOK_Default;
                case GameMode.TFT:
                    return TFT_Default;
                case GameMode.URF:
                    return URF_Default;
                case GameMode.ARURF:
                    return ARURF_Default;
            }
            return DefaultSource;
        }

        public void resetDefaultConfig(GameMode gm)
        {
            switch (gm)
            {
                case GameMode.ARAM:
                    ARAM_Default = DefaultSource;
                    break;
                case GameMode.CLASSIC:
                    CLASSIC_Default = DefaultSource;
                    break;
                case GameMode.PRACTICETOOL:
                    PRACTICETOOL_Default = DefaultSource;
                    break;
                case GameMode.ULTBOOK:
                    ULTBOOK_Default = DefaultSource;
                    break;
                case GameMode.TFT:
                    TFT_Default = DefaultSource;
                    break;
                case GameMode.URF:
                    URF_Default = DefaultSource;
                    break;
                case GameMode.ARURF:
                    ARURF_Default = DefaultSource;
                    break;
            }
        }

        public void setDefaultConfig(GameMode gm, string config)
        {
            switch (gm)
            {
                case GameMode.ARAM:
                    ARAM_Default = config;
                    break;
                case GameMode.CLASSIC:
                    CLASSIC_Default = config;
                    break;
                case GameMode.PRACTICETOOL:
                    PRACTICETOOL_Default = config;
                    break;
                case GameMode.ULTBOOK:
                    ULTBOOK_Default = config;
                    break;
                case GameMode.TFT:
                    TFT_Default = config;
                    break;
                case GameMode.URF:
                    URF_Default = config;
                    break;
                case GameMode.ARURF:
                    ARURF_Default = config;
                    break;
            }
        }
    }
}
