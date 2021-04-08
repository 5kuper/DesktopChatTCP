using System;
using Packets;

namespace Client.Models
{
    public static class PacketHandler
    {
        public static void Handle(Packet packet)
        {
            if (packet is MessagePacket)
            {

            }
            else if (packet is WarningPacket)
            {

            }
            else
            {
                throw new NotImplementedException("No handler defined for this type of packet!");
            }
        }
    }
}
