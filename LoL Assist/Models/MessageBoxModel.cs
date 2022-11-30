using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoL_Assist_WAPP.Models
{
    public class MessageBoxModel
    {
        public Action Action { get; set; }
        public string Message { get; set; }
        public string Title { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
    }
}
