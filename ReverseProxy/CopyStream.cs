using System;
using System.Net.Sockets;

namespace ReverseProxy
{
    public class CopyStream
    {
        private readonly NetworkStream m_destStream;
        private readonly TcpClient m_destClient;
        private readonly Action m_onClose;
        private readonly NetworkStream m_sourceStream;
        private readonly TcpClient m_sourceClient;

        public CopyStream( TcpClient source, TcpClient dest, Action onClose )
        {
            m_destClient = dest;
            m_onClose = onClose;
            m_sourceClient = source;

            m_sourceStream = m_sourceClient.GetStream();
            m_destStream = m_destClient.GetStream();

            var sourceBytes = new byte[2048];
            m_sourceStream.BeginRead( sourceBytes, 0, sourceBytes.Length, EndSourceRead, sourceBytes );
        }

        private void EndSourceRead( IAsyncResult ar )
        {
            try
            {
                var sourceBytes = (byte[])ar.AsyncState;

                var read = m_sourceStream.EndRead( ar );

                if( read > 0 )
                {
                    m_destStream.BeginWrite( sourceBytes, 0, read, EndDestWrite, null );
                    m_sourceStream.BeginRead( sourceBytes, 0, sourceBytes.Length, EndSourceRead, sourceBytes );
                }
                else
                {
                    m_sourceStream.Close();
                    m_sourceClient.Close();

                    m_onClose();
                }
            }
            catch( Exception ex )
            {
                m_sourceStream.Close();
                m_sourceClient.Close();

                m_onClose();
            }
        }

        private void EndDestWrite( IAsyncResult ar )
        {
            try
            {
                m_destStream.EndWrite( ar );
            }
            catch( Exception ex )
            {
                m_sourceStream.Close();
                m_sourceClient.Close();

                m_onClose();
            }
        }
    }
}