using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using Packets;

namespace Server
{
    public class Server
    {
        public int Port { get; }
        public int MaxConnections { get; }

        private readonly TcpListener _listener;
        private readonly List<Connection> _connections;

        public void AddConnection(Connection connection) => _connections.Add(connection);
        public void RemoveConnection(Connection connection) => _connections.Remove(connection);

        public Server(int port, int maxConnections = -1)
        {
            Port = port;
            MaxConnections = maxConnections;

            _listener = new TcpListener(IPAddress.Any, port);
            _connections = new List<Connection>();
        }

        public void Log(string text)
        {
            Console.WriteLine($"{DateTime.Now}: {text}");
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
                Log($"Server failed to start - {e.Message}");
                Stop();
            }
        }
        private void ConnectCallback(IAsyncResult result)
        {
            try
            {
                TcpClient client = _listener.EndAcceptTcpClient(result);
                _listener.BeginAcceptTcpClient(ConnectCallback, null);

                Log($"Incoming connection from {client.Client.RemoteEndPoint}...");

                if (MaxConnections > 0 && _connections.Count >= MaxConnections)
                {
                    Log($"{client.Client.RemoteEndPoint} failed to connect - Server full!");
                }
                else
                {
                    Connection connection = new Connection(client, this);
                    connection.Begin();
                    Log($"Client {client.Client.RemoteEndPoint} connected to server.");
                }
            }
            catch (Exception e)
            {
                if (e is ObjectDisposedException)
                {
                    return; // Server stopped
                }

                Log($"Server failed to connect new client - {e.Message}");
                Stop();
            }
        }

        public void BroadcastPacket(Packet packet, Connection ignored)
        {
            foreach (var client in _connections.Where(client => client != ignored))
            {
                client.SendPacket(packet);
            }
        }

        public void Stop()
        {
            _listener.Stop();
            foreach (var connection in _connections)
            {
                connection.Close();
            }
            Log("Server stopped.");
        }
    }
}
