using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoLA.Networking.WebWrapper.DataProviders.OPGG
{

    public class OPGGChampionData
    {
        public Pageprops pageProps { get; set; }
    }

    public class Pageprops
    {
        public string champion { get; set; }
        public string position { get; set; }
        public string region { get; set; }
        public string tier { get; set; }
        public Data data { get; set; }
    }

    public class Data
    {
        public Summoner_Spells[] summoner_spells { get; set; }
        public Trends trends { get; set; }
        public Skill[] skills { get; set; }
        public Skill_Masteries[] skill_masteries { get; set; }
        public object[] skill_evolves { get; set; }
        public OPRune[] runes { get; set; }
        public Core_Items[] core_items { get; set; }
        public Boot[] boots { get; set; }
        public Starter_Items[] starter_items { get; set; }
        public Last_Items[] last_items { get; set; }
    }

    public class Trends
    {
        public int total_rank { get; set; }
        public int total_position_rank { get; set; }
        public Win[] win { get; set; }
        public Pick[] pick { get; set; }
        public Ban[] ban { get; set; }
    }

    public class Win
    {
        public string version { get; set; }
        public float rate { get; set; }
        public int rank { get; set; }
    }

    public class Pick
    {
        public string version { get; set; }
        public float rate { get; set; }
        public int rank { get; set; }
    }

    public class Ban
    {
        public string version { get; set; }
        public float rate { get; set; }
        public int rank { get; set; }
    }

    public class Summoner_Spells
    {
        public int[] ids { get; set; }
        public int win { get; set; }
        public int play { get; set; }
        public float pick_rate { get; set; }
    }

    public class Skill
    {
        public string[] order { get; set; }
        public int play { get; set; }
        public int win { get; set; }
        public float pick_rate { get; set; }
    }

    public class Skill_Masteries
    {
        public string[] ids { get; set; }
        public int play { get; set; }
        public int win { get; set; }
        public float pick_rate { get; set; }
        public Build[] builds { get; set; }
    }

    public class Build
    {
        public string[] order { get; set; }
        public int play { get; set; }
        public int win { get; set; }
        public float pick_rate { get; set; }
    }

    public class OPRune
    {
        public int id { get; set; }
        public int primary_page_id { get; set; }
        public int[] primary_rune_ids { get; set; }
        public int secondary_page_id { get; set; }
        public int[] secondary_rune_ids { get; set; }
        public int[] stat_mod_ids { get; set; }
        public int play { get; set; }
        public int win { get; set; }
        public float pick_rate { get; set; }
    }

    public class Core_Items
    {
        public int[] ids { get; set; }
        public int play { get; set; }
        public int win { get; set; }
        public float pick_rate { get; set; }
    }

    public class Boot
    {
        public int[] ids { get; set; }
        public int play { get; set; }
        public int win { get; set; }
        public float pick_rate { get; set; }
    }

    public class Starter_Items
    {
        public int[] ids { get; set; }
        public int play { get; set; }
        public int win { get; set; }
        public float pick_rate { get; set; }
    }

    public class Last_Items
    {
        public int[] ids { get; set; }
        public int play { get; set; }
        public int win { get; set; }
        public float pick_rate { get; set; }
    }
}
