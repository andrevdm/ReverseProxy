using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ReverseProxy
{
    public class PortForwarder
    {
        public Guid Id { get; private set; }
        private bool m_started = false;
        public PortMap Map { get; set; }
        public TcpListener Server { get; set; }
        public Task ServerTask { get; private set; }
        public ConcurrentDictionary<Guid, ClientInfo> Clients { get; set; }

        public PortForwarder( PortMap map )
        {
            Id = Guid.NewGuid();
            Clients = new ConcurrentDictionary<Guid, ClientInfo>();

            Map = map;
            Server = new TcpListener( IPAddress.Any, map.FromPort );
        }

        public void StartServer()
        {
            if( m_started )
            {
                return;
            }

            m_started = true;
            ServerTask = Task.Factory.StartNew( RunServer );
        }

        private void RunServer()
        {
            Server.Start();
            Server.BeginAcceptTcpClient( EndAcceptSocketTcpClient, null );
        }

        private void EndAcceptSocketTcpClient( IAsyncResult ar )
        {
            var client = Server.EndAcceptTcpClient( ar );

            var id = Guid.NewGuid();
            var info = new ClientInfo
                {
                    Id = id,
                    SourceClient = client,
                };

            Clients[id] = info;

            Task.Factory.StartNew( () => HandleClient( info ) );

            Server.BeginAcceptTcpClient( EndAcceptSocketTcpClient, null );
        }

        private void HandleClient( ClientInfo info )
        {
            var destClient = new TcpClient();
            info.DestClient = destClient;

            var toEndPoint = Map.GetNextEndPoint();
            destClient.BeginConnect( toEndPoint.Address, toEndPoint.Port, EndConnectWriter, info );
        }

        private void EndConnectWriter( IAsyncResult ar )
        {
            var info = (ClientInfo)ar.AsyncState;
            info.DestClient.EndConnect( ar );
            info.SourceToDest = new CopyStream( info.SourceClient, info.DestClient, () => Close( info.Id ) );
            info.DestToSource = new CopyStream( info.DestClient, info.SourceClient, () => Close( info.Id ) );
        }

        private void Close( Guid id )
        {
            ClientInfo info;

            if( Clients.TryRemove( id, out info ) )
            {
                if( info.SourceClient.Connected )
                {
                    info.SourceClient.Close();
                }

                if( info.DestClient.Connected )
                {
                    //info.DestClient.Close();
                }
            }
        }
    }
}