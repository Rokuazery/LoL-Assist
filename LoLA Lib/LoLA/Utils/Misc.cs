using System.Text;
using System.Linq;
using System.IO;
using System;

namespace LoLA.Utils
{
    public static class Misc
    {
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
            using (StreamReader streamReader = new StreamReader(stream))
            {
                return streamReader.ReadToEnd();
            }
        }
        public static string Base64Encode(string plainText, int multiplier = 1)
        {
            string temp = plainText;
            for (int i = 0; i < multiplier; i++)
            {
                var plainTextBytes = Encoding.UTF8.GetBytes(temp);
                temp = Convert.ToBase64String(plainTextBytes);
            }
            return temp;
        }
        public static string Base64Decode(string base64EncodedData, int multiplier = 1)
        {
            string temp = base64EncodedData;
            for (int i = 0; i < multiplier; i++)
            {
                var base64EncodedBytes = Convert.FromBase64String(temp);
                temp = Encoding.UTF8.GetString(base64EncodedBytes);
            }
            return temp;
        }
    }
}
