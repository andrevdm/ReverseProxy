using System;
using System.Collections.Generic;
using System.Net;

namespace ReverseProxy
{
    internal class Program
    {
        private static void Main( string[] args )
        {
            List<ReverseProxy> proxies = ReverseProxy.LoadFromConfig();

            foreach( var proxy in proxies )
            {
                foreach( PortForwarder forwarder in proxy.Forwarders )
                {
                    Console.WriteLine( "From {0}", forwarder.Map.FromPort );
                    Console.WriteLine( "To" );

                    foreach( var map in forwarder.Map.Endpoints )
                    {
                        Console.WriteLine( "   {0}:{1}", map.Address, map.Port );
                    }
                }

                Console.WriteLine();
            }

            Console.WriteLine( "Press any key to exit" );
            Console.ReadLine();
        }
    }
}
