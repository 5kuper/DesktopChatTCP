using System;

namespace Packets
{
    // Username change request from client to server

    [Packet(5, typeof(RenamingRequestPacket)), Serializable]
    public class RenamingRequestPacket : Packet
    {
        public string NewUsername { get; set; }

        public RenamingRequestPacket() { }

        public RenamingRequestPacket(string username)
        {
            NewUsername = username;
        }
    }
}
