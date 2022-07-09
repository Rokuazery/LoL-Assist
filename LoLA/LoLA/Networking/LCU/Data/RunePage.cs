using System.Collections.Generic;

namespace LoLA.Networking.LCU.Objects
{
    public class RunePage
    {
        public List<int> autoModifiedSelections { get; set; }
        public bool current { get; set; }
        public ulong id { get; set; }
        public bool isActive { get; set; } // Disini cek buat aktif atau tidaknya
        public bool isDeletable { get; set; }
        public bool isEditable { get; set; } // Disini buat ngecek bisa diedit atau ngk
        public bool isValid { get; set; }
        public long lastModified { get; set; }
        public string name { get; set; }
        public int order { get; set; }
        public int primaryStyleId { get; set; }
        public List<int> selectedPerkIds { get; set; }
        public int subStyleId { get; set; }
    }
}
