using LoLA.Data.Enums;
using System;

namespace LoL_Assist_WAPP.Converters
{
    public static class ProviderConverter
    {
        // Test
        public static string ToName(Provider provider)
            => provider switch {
                Provider.UGG => "U.GG",
                Provider.METAsrc => "METAsrc.com",
                _ => provider.ToString()
            };

        public static Provider ToEnum(string provider)
            => provider switch
            {
                "U.GG" => Provider.UGG,
                "METAsrc.com" => Provider.METAsrc,
                _ => (Provider)Enum.Parse(typeof(Provider), provider)
            };
    }
}
