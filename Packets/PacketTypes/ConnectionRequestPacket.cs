using System;

namespace Packets
{
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
