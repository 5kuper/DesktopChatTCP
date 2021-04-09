using System;
using System.Net.Sockets;
using Packets;

namespace ClientSide.Models
{
    public class Client
    {
        public string ID { get; set; }
        public string Username { get; set; }

        private TcpClient _socket;
        private NetworkStream _stream;

        private readonly byte[] _buffer = new byte[4096];

        public Action<string> Log { get; private set; } = s => { };
        public event Action<string> OnLog
        {
            add => Log += value;
            remove => Log -= value;
        }

        public void Connect(string host, int port)
        {
            _socket = new TcpClient();
            _socket.BeginConnect(host, port, ConnectCallback, _socket);
        }

        public void ConnectCallback(IAsyncResult result)
        {
            try
            {
                _socket.EndConnect(result);
            }
            catch (Exception e)
            {
                Log($"Failed to connect to server: {e.Message}");
            }

            if (!_socket.Connected)
            {
                return;
            }

            _stream = _socket.GetStream();
            _stream.BeginRead(_buffer, 0, _buffer.Length, ReceiveCallback, null);
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
                    Log("Received data that cannot be deserialized to a packet!");
                }

                _stream.BeginRead(_buffer, 0, _buffer.Length, ReceiveCallback, null);
            }
            catch (Exception e)
            {
                Log($"Failed to receive data from server: {e.Message}");
            }
        }

        public void SendPacket(Packet packet)
        {
            try
            {
                packet.Serialize(out byte[] data);
                _stream?.WriteAsync(data, 0, data.Length);
            }
            catch (Exception e)
            {
                Log($"Failed to send data to server: {e.Message}");
            }
        }

        public void Disconnect()
        {
            _stream?.Close();
            _socket?.Close();
        }
    }
}
