using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoL_Assist_WAPP.Models
{
    public class ProgressReportModel
    {
        public int Percent { get; set; } = 0;
        public string Status { get; set; }
        public string DownloadedTotal { get; set; }
    }
}
