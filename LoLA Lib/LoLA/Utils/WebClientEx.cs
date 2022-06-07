using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;

namespace LoLA.Utils
{
    public class WebClientEx : WebClient
    {
        public int Timeout { get; set; }

        protected override WebRequest GetWebRequest(Uri uri)
        {
            WebRequest lWebRequest = base.GetWebRequest(uri);
            lWebRequest.Timeout = Timeout;
            ((HttpWebRequest)lWebRequest).ReadWriteTimeout = Timeout;
            return lWebRequest;
        }
    }
}
