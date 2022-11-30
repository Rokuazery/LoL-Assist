using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoLA.Data
{
    public class Block
    {
        public List<Item> items { get; set; }
        public string type { get; set; }
    }

    public class Item
    {
        public string id { get; set; }
        public int count { get; set; }
    }

    public class ItemSet
    {
        public string title { get; set; }
        public List<int> associatedMaps { get; set; }
        public List<int> associatedChampions { get; set; }
        public List<Block> blocks { get; set; }
    }
}
