using System.Collections.Generic;

namespace ReverseProxy
{
    public class ReverseProxyConfig
    {
        public int HostPort { get; set; }
        public List<PortForwardConfig> ForwardTo { get; set; }
    }
}