using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoLA.Networking.WebWrapper.DataDragon.Data
{
    public class Perk
    {
        public int id { get; set; }
        public string key { get; set; }
        public string icon { get; set; }
        public string name { get; set; }
        public List<Slots> slots = new List<Slots>();
    }

    public class Slots
    {
        public List<RuneInfo> runes = new List<RuneInfo>();
    }
}
