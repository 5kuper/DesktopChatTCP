using System;
using System.Net;
using System.Net.Sockets;
using Packets;

namespace ServerSide
{
    public class Connection
    {
        public bool IsConfirmed { get; set; }

        public string ID { get; set; }
        public string Username { get; set; }

        public EndPoint RemoteEndPoint { get; }

        private readonly Server _server;
        private readonly TcpClient _socket;
        private readonly NetworkStream _stream;

        private readonly byte[] _buffer;

        public Connection(TcpClient client, Server server)
        {
            _server = server;
            _socket = client;
            _stream = client.GetStream();

            _buffer = new byte[4096];

            RemoteEndPoint = _socket.Client.RemoteEndPoint;
        }

        public void Begin()
        {
            _server.ProcessingClients.Add(this);
            _stream.BeginRead(_buffer, 0, _buffer.Length, ReceiveCallback, null);
        }

        private void ReceiveCallback(IAsyncResult result)
        {
            try
            {
                int byteLength = _stream.EndRead(result);
                byte[] data = new byte[byteLength];
                Array.Copy(_buffer, data, byteLength);

                try
                {
                    Packet packet = Packet.Deserialize(data);

                    if (!IsConfirmed && packet is not ConnectionRequestPacket)
                    {
                        _server.Log($"Closing unconfirmed connection {RemoteEndPoint} " +
                                    $"from which data was received that isn't a ConnectionRequestPacket!");
                        Close();
                        return;
                    }

                    _server.Handler.Handle(packet, this);
                    _stream.BeginRead(_buffer, 0, _buffer.Length, ReceiveCallback, null);
                }
                catch (Exception e)
                {
                    if (e is ObjectDisposedException)
                    {
                        return; // Connection closed
                    }

                    _server.Log($"Closing connection {RemoteEndPoint} from which data was received that cannot be deserialized to a packet!");
                    Close();
                }
            }
            catch (Exception e)
            {
                if (e is ObjectDisposedException)
                {
                    return; // Connection closed
                }

                _server?.Log($"Closing connection {RemoteEndPoint} from which failed to receive data:\n{e.Message}");
                Close();
            }
        }

        public void SendPacket(Packet packet)
        {
            packet.Serialize(out byte[] data);
            try
            {
                _stream.WriteAsync(data, 0, data.Length);
            }
            catch (Exception e)
            {
                if (e is ObjectDisposedException)
                {
                    return; // Connection closed
                }

                _server?.Log($"Closing connection {RemoteEndPoint} that failed to send data:\n{e.Message}");
                Close();
            }
        }

        public void Close()
        {
            _server.Log($"Connection {RemoteEndPoint} closed.");

            _server.ConnectedUsers.Remove(this);
            _server.ProcessingClients.Remove(this);

            _stream?.Close();
            _socket?.Close();
        }
    }
}
