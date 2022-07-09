using System.Reflection;
using System.IO;

namespace LoLA
{
    public static class LibInfo
    {
        public const string NAME = "LoLA";
        public static readonly string  r_LibFolderPath = $"{Path.GetTempPath()}\\{NAME} Data";
        public static readonly string r_Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
    }
}
