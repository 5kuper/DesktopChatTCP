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
                _client.DisplayMessage(message.Sender, message.Content);
            }
            else if (packet is NotificationPacket notification)
            {
                switch (notification.Code)
                {
                    case NotificationCode.ServerStopping:
                        _client.Disconnect();
                        _client.Log("Server stopped!");
                        break;
                    case NotificationCode.UserConnected:
                        _client.Log($"User \"{notification.Content[0]}\" connected to the party.");
                        break;
                    case NotificationCode.UserDisconnected:
                        _client.Log($"User \"{notification.Content[0]}\" disconnected from the party.");
                        break;
                    case NotificationCode.UserRenamed:
                        _client.Log($"User \"{notification.Content[0]}\" renamed to \"{notification.Content[1]}\".");
                        break;
                    case NotificationCode.UserKicked:
                        string username = notification.Content[0];
                        _client.Log($"User \"{username}\" kicked by the server!");
                        if (username == _client.Username)
                        {
                            _client.Disconnect();
                        }
                        break;
                    /* default:
                        _client.Log($"Server sent invalid notification!");
                        break; */
                }
            }
            else if (packet is ConnectionResponsePacket response)
            {
                if (response.ConnectionConfirmed)
                {
                    _client.ID = response.UserID;
                    _client.Log($"You have connected to the server as \"{_client.Username}\".");
                    _client.RaiseConnectionStatusChanged();
                }
                else
                {
                    _client.RaiseConnectionStatusChanged();
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
                        default:
                            _client.Log("Failed to connect: Unknown reason...");
                            break;
                    }
                }
            }
            else if (packet is RenamingResponsePacket renaming)
            {
                if (renaming.RenamingConfirmed)
                {
                    _client.Log("Renaming request was accepted!");
                    _client.Username = renaming.NewUsername;
                }
                else
                {
                    switch (renaming.RejectionReason)
                    {
                        case RejectionReason.UsernameUnavailable:
                            _client.Log("Renaming request was rejected: Username unavailable!");
                            break;
                        case RejectionReason.UsernameAlreadyUsed:
                            _client.Log("Renaming request was rejected: Username already used!");
                            break;
                        default:
                            _client.Log("Renaming request was rejected: Unknown reason...");
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
