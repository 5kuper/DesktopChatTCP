using System;
using Packets;

namespace Server
{
    public static class PacketHandler
    {
        public static void Handle(Packet packet)
        {
            if (packet is MessagePacket)
            {
                MessagePacket message = (MessagePacket)packet;
                Program.Log($"User {message.Sender} said \"{message.Content}\"");
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
