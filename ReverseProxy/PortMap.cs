using System.Net;
using System.Threading;

namespace ReverseProxy
{
    public class PortMap
    {
        private int m_at = 0;

        public PortMap( int fromPort, IPEndPoint[] endpoints )
        {
            FromPort = fromPort;
            Endpoints = endpoints;
        }

        public int FromPort { get; private set; }
        public IPEndPoint[] Endpoints { get; set; }

        public IPEndPoint GetNextEndPoint()
        {
            if( m_at > int.MaxValue - 1 )
            {
                m_at = 0;
            }

            var at = Interlocked.Increment( ref m_at );
            return Endpoints[at % Endpoints.Length];
        }
    }
}