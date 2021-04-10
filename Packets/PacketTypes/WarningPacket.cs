using System;

namespace Packets
{
    public enum WarningCode
    {
        // Warnings from a server
        ServerStopping,
        // Warnings from a client
        ClientDisconnecting,
    }

    [Packet(2, typeof(WarningPacket)), Serializable] 
    public class WarningPacket : Packet
    {
        public WarningCode Code { get; set; }

        public WarningPacket() { }

        public WarningPacket(WarningCode code)
        {
            Code = code;
        }
    }
}
