using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HLA.Integration.Innov8tif.WebServer.Domain
{
    public class ProxyConfig
    {
        public bool isEnable { get; set; }
        public string? Address { get; set; }
        public bool BypassProxyOnLocal { get; set; }
        public bool UseDefaultCredentials { get; set; }
    }
}
