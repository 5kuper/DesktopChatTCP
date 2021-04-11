using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using Packets;

namespace ServerSide
{
    public class Server
    {
        public int Port { get; }
        public int MaxConnections { get; }

        public PacketHandler Handler { get; }

        public List<Connection> ConnectedUsers { get; } // Confirmed connections
        public List<Connection> ProcessingClients { get; } // Unconfirmed connections

        private readonly TcpListener _listener;

        public Action<string> Log { get; private set; } = s => { };
        public event Action<string> OnLog
        {
            add => Log += value;
            remove => Log -= value;
        }

        public Server(int port, int maxConnections = -1)
        {
            Port = port;
            MaxConnections = maxConnections;

            Handler = new PacketHandler(this);

            ConnectedUsers = new List<Connection>();
            ProcessingClients = new List<Connection>();

            _listener = new TcpListener(IPAddress.Any, port);
        }

        public void Start()
        {
            try
            {
                Log("Starting server...");

                _listener.Start();
                _listener.BeginAcceptTcpClient(ConnectCallback, null);

                Log($"Server started on port {Port}.");
            }
            catch (Exception e)
            {
                Log($"Server failed to start:\n{e.Message}");
                Stop();
            }
        }

        private void ConnectCallback(IAsyncResult result)
        {
            try
            {
                TcpClient client = _listener.EndAcceptTcpClient(result);
                _listener.BeginAcceptTcpClient(ConnectCallback, null);

                Connection connection = new Connection(client, this);
                connection.Begin();

                Log($"Processing incoming connection from {client.Client?.RemoteEndPoint}...");

                if (MaxConnections > 0 && ConnectedUsers.Count >= MaxConnections)
                {
                    connection.SendPacket(new ConnectionResponsePacket(RejectionReason.ServerFull));
                    Log($"Connection request from {client.Client?.RemoteEndPoint} was rejected: Server full!");
                    connection.Close();
                }
            }
            catch (Exception e)
            {
                if (e is ObjectDisposedException)
                {
                    return; // Server stopped
                }

                Log($"Server failed to connect new client:\n{e.Message}");
                Stop();
            }
        }

        public void BroadcastPacket(Packet packet, Connection ignored = null)
        {
            foreach (var client in ConnectedUsers.Where(client => client != ignored))
            {
                client.SendPacket(packet);
            }
        }

        public void Stop()
        {
            BroadcastPacket(new NotificationPacket(NotificationCode.ServerStopping));

            _listener.Stop();
            foreach (var connection in ConnectedUsers)
            {
                connection.Close();
            }
            Log("Server stopped.");
        }
    }
}
