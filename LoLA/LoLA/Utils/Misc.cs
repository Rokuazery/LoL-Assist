using System.Text.RegularExpressions;
using System.Linq;
using System.IO;
using System;

namespace LoLA.Utils
{
    public static class Misc
    {
        public static string StripHTML(string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty);
        }

        public static string FixedName(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                if (name.Contains("'"))
                    return name;

                return name + "'s";
            }
            return "Jeff's";
        }

        public static string Normalize(string input)
        {
            return input.ToLower().First().ToString()
            .ToUpper() + string.Join("", input.Skip(1));
        }

        public static string ReadStream(Stream stream)
        {
            using StreamReader streamReader = new StreamReader(stream);
            return streamReader.ReadToEnd();
        }
    }
}
