using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoLA.Networking.WebWrapper.DataDragon.Data
{
    public class Passive
    {
        public string name { get; set; }
        public string description { get; set; }
        public Image image = new Image();
    }
}
