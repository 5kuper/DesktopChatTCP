using System;
using System.Net.Sockets;
using Packets;

namespace Server
{
    public class Connection
    {
        public string ID { get; }
        public string Username { get; }

        private readonly Server _server;
        private readonly TcpClient _socket;
        private readonly NetworkStream _stream;

        private byte[] _buffer;

        public Connection(TcpClient client, Server server)
        {
            ID = Guid.NewGuid().ToString();

            _server = server;
            _socket = client;
            _stream = client.GetStream();

            _buffer = new byte[4096];
        }

        public void Begin()
        {
            _server.AddConnection(this);
            _stream.BeginRead(_buffer, 0, _buffer.Length, ReceiveCallback, null);
        }

        private void ReceiveCallback(IAsyncResult result)
        {
            try
            {
                int byteLength = _stream.EndRead(result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void SendPacket(Packet packet)
        {
            try
            {
                packet.Serialize(out byte[] data);
                _stream.WriteAsync(data, 0, data.Length);
            }
            catch (Exception e)
            {
                _server.Log($"Failed to send data to {_socket.Client.RemoteEndPoint}: - {e.Message}");
            }
        }

        public void Close()
        {
            _server.RemoveConnection(this);

            _stream?.Close();
            _socket?.Close();
        }
    }
}
