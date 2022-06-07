using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoLA.LCU.Objects
{
    public class Matchmaking
    {
        public string dodgeWarning { get; set; }
        public string playerResponse { get; set; } = "None";
        public string state { get; set; }
        public bool suppressUx { get; set; }
        public  double timer { get; set; }
    }
}
