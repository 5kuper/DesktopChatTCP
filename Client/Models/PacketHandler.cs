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
                        _client.Disconnect();
                        _client.Log($"Server stopped!");
                        break;
                    /* default:
                        _client.Log($"Server sent invalid warning!");
                        break; */
                }
            }
            else if (packet is ConnectionResponsePacket response)
            {
                if (response.ConnectionConfirmed)
                {
                    _client.ID = response.UserID;
                }
                else
                {
                    switch (response.RejectionReason)
                    {
                        case RejectionReason.ServerFull:
                            _client.Log("Failed to connect: Server full!");
                            break;
                        case RejectionReason.UsernameUnavailable:
                            _client.Log("Failed to connect: Username unavailable!");
                            break;
                        case RejectionReason.UsernameAlreadyUsed:
                            _client.Log("Failed to connect: Username already used!");
                            break;
                    }
                }
            }
            else
            {
                throw new NotImplementedException("No handler defined for this type of packet!");
            }
        }
    }
}
