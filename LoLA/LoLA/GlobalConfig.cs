namespace LoLA
{
    public static class GlobalConfig
    {
        public static bool s_Debug { get; set; } = false;
        public static bool s_Caching { get; set; } = true;
        public static bool s_Logging { get; set; } = true;
        public static bool s_LatestPatch { get; set; } = false;
        public static string s_DataDragonPatch { get; set; } = "latest";
    }
}
