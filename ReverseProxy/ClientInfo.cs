using System;
using System.Net.Sockets;

namespace ReverseProxy
{
    public class ClientInfo
    {
        public Guid Id { get; set; }
        public TcpClient DestClient { get; set; }
        public TcpClient SourceClient { get; set; }
        public CopyStream SourceToDest { get; set; }
        public CopyStream DestToSource { get; set; }
    }
}