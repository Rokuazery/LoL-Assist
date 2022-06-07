using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoLA.Utils.Logger
{
    public class LogModel
    {
        public LogType type { get; set; }
        public string source { get; set; }
        public string message { get; set; }
    }
}
