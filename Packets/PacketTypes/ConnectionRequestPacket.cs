using System;

namespace Packets
{
    // Clients send it to the server after connecting

    [Packet(3, typeof(ConnectionRequestPacket)), Serializable]
    public class ConnectionRequestPacket : Packet
    {
        public string Username { get; set; }

        public ConnectionRequestPacket() { }

        public ConnectionRequestPacket(string username)
        {
            Username = username;
        }
    }
}
