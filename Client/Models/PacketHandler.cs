using System;
using Packets;

namespace ClientSide.Models
{
    /// <summary>It handles packets sent to the client.</summary>
    public class PacketHandler
    {
        private readonly Client _client;

        public PacketHandler(Client client) => _client = client;

        public void Handle(Packet packet)
        {
            if (packet is MessagePacket message)
            {
                _client.WriteMessage(message.Sender, message.Content);
            }
            else if (packet is WarningPacket warning)
            {
                switch (warning.Code)
                {
                    case WarningCode.ServerStopping:
                        _client.Log($"Server stopped!");
                        break;
                    /* default:
                        _client.Log($"Server sent invalid warning!");
                        break; */
                }
            }
            else
            {
                throw new NotImplementedException("No handler defined for this type of packet!");
            }
        }
    }
}
