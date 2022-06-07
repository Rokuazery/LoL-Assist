using System.Reflection;
using System.IO;


namespace LoLA
{
    public static class Global
    {
        public const string name = "LoLA";
        public const string defaultPatch = "11.15.1";
        public static string libraryFolder = $"{Path.GetTempPath()}\\{name}";
        public static string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public static class Config
        {
            public static bool debug = false;
            public static bool caching = true;
            public static bool logging = true;
            public static bool useLatestPatch = false;
            public static string dDragonPatch = defaultPatch;
        }
    }
}
