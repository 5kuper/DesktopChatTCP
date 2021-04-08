using System;
using System.Net.Sockets;
using Packets;

namespace Client.Models
{
    public class Client
    {
        public string ID { get; set; }
        public string Username { get; set; }

        private TcpClient _socket;
        private NetworkStream _stream;

        private byte[] _buffer;

        public void Connect(string host, int port)
        {
            _socket = new TcpClient();
            _socket.BeginConnect(host, port, ConnectCallback, _socket);
            _buffer = new byte[4096];
        }

        public void ConnectCallback(IAsyncResult result)
        {
            _socket.EndConnect(result);

            if (!_socket.Connected)
            {
                return;
            }

            _stream = _socket.GetStream();
            _stream.BeginRead(_buffer, 0, _buffer.Length, ReceiveCallback, null);

            SendPacket(new MessagePacket("Skuper", "Hello")); // TODO: Delete it
        }

        private void ReceiveCallback(IAsyncResult result)
        {
            try
            {
                int byteLength = _stream.EndRead(result);
                if (byteLength <= 0)
                {
                    return;
                }

                byte[] data = new byte[byteLength];
                Array.Copy(_buffer, data, byteLength);

                try
                {
                    Packet packet = Packet.Deserialize(data);
                    PacketHandler.Handle(packet);
                }
                catch
                {
                    // TODO: Log("Received data that cannot be deserialized to a packet!");
                }

                _stream.BeginRead(_buffer, 0, _buffer.Length, ReceiveCallback, null);
            }
            catch (Exception e)
            {
                // TODO: Log($"Failed to receive data from server - {e.Message}");
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
                // TODO: Log($"Failed to send data to server - {e.Message}");
            }
        }

        public void Disconnect()
        {
            _stream?.Close();
            _socket?.Close();
        }
    }
}
