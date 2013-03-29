using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;

namespace ReverseProxy
{
    public class ReverseProxy
    {
        public ReverseProxy()
        {
            ForwarderMap = new Dictionary<int, PortForwarder>();
        }

        public void AddForwarder( int fromPort, IPEndPoint[] endpoints )
        {
            var map = new PortMap( fromPort, endpoints );
            var forwarder = new PortForwarder( map );
            ForwarderMap.Add( fromPort, forwarder );
            forwarder.StartServer();
        }

        public Dictionary<int, PortForwarder> ForwarderMap { get; private set; }

        public IEnumerable<PortForwarder> Forwarders
        {
            get { return ForwarderMap.Values; }
        }

        public static List<ReverseProxy> LoadFromConfig()
        {
            var config = (ReverseProxiesConfig)ConfigurationManager.GetSection( "ReverseProxiesConfig" );

            var proxies = new List<ReverseProxy>();

            foreach( ReverseProxyConfig proxyConfig in config.Proxies )
            {
                var proxy = new ReverseProxy();
                proxy.AddForwarder( proxyConfig.HostPort, GetEndpoints( proxyConfig.ForwardTo ) );
                proxies.Add( proxy );
            }

            return proxies;
        }

        private static IPEndPoint[] GetEndpoints( IEnumerable<PortForwardConfig> forwardTo )
        {
            return (from f in forwardTo
                    select new IPEndPoint( IPAddress.Parse( f.ForwardToIp ), f.ForwardToPort )).ToArray();
        }
    }
}