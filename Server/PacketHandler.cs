using System;
using System.Collections.Generic;
using System.Linq;
using Packets;

namespace ServerSide
{
    /// <summary>It handles packets sent to the server.</summary>
    public class PacketHandler
    {
        private static readonly List<string> UnavailableUsernames = new List<string>() {"You"};

        private readonly Server _server;

        public PacketHandler(Server server) => _server = server;

        public void Handle(Packet packet, Connection sender)
        {
            if (packet is MessagePacket message)
            {
                _server.Log($"User \"{sender.Username}\" said: \"{message.Content}\"");
                _server.BroadcastPacket(new MessagePacket(sender.Username, message.Content), sender);
            }
            else if (packet is NotificationPacket notification)
            {
                switch (notification.Code)
                {
                    case NotificationCode.ClientDisconnecting:
                        sender.Close();
                        _server.Log($"User \"{sender.Username}\" disconnected!");
                        _server.BroadcastPacket(new NotificationPacket(NotificationCode.UserDisconnected, sender.Username));
                        break;
                    default:
                        _server.Log($"User \"{sender.Username}\" sent invalid notification!");
                        break;
                }
            }
            else if (packet is ConnectionRequestPacket request)
            {
                if (UnavailableUsernames.Contains(request.Username))
                {
                    sender.SendPacket(new ConnectionResponsePacket(RejectionReason.UsernameUnavailable));
                    _server.Log($"Connection request from {sender.RemoteEndPoint} was rejected: Username \"{request.Username}\" unavailable.");
                    sender.Close();
                }
                else if (_server.ConnectedUsers.Any(c => c.Username == request.Username))
                {
                    sender.SendPacket(new ConnectionResponsePacket(RejectionReason.UsernameAlreadyUsed));
                    _server.Log($"Connection request from {sender.RemoteEndPoint} was rejected: Username \"{request.Username}\" already used.");
                    sender.Close();
                }
                else
                {
                    _server.ProcessingClients.Remove(sender);
                    _server.ConnectedUsers.Add(sender);

                    sender.IsConfirmed = true;
                    sender.ID = Guid.NewGuid().ToString();
                    sender.Username = request.Username;

                    sender.SendPacket(new ConnectionResponsePacket(sender.ID));

                    _server.Log($"Client {sender.RemoteEndPoint} connected to server as \"{sender.Username}\".");
                    _server.BroadcastPacket(new NotificationPacket(NotificationCode.UserConnected, sender.Username), sender);
                }
            }
            else if (packet is RenamingRequestPacket renaming)
            {
                if (UnavailableUsernames.Contains(renaming.NewUsername))
                {
                    sender.SendPacket(new RenamingResponsePacket(RejectionReason.UsernameUnavailable));
                    _server.Log($"Renaming request from {sender.Username} was rejected: Username \"{renaming.NewUsername}\" unavailable.");
                }
                else if (_server.ConnectedUsers.Any(c => c.Username == renaming.NewUsername))
                {
                    sender.SendPacket(new RenamingResponsePacket(RejectionReason.UsernameAlreadyUsed));
                    _server.Log($"Renaming request from {sender.Username} was rejected: Username \"{renaming.NewUsername}\" already used.");
                }
                else
                {
                    sender.SendPacket(new RenamingResponsePacket(renaming.NewUsername));
                    _server.BroadcastPacket(new NotificationPacket(NotificationCode.UserRenamed, sender.Username, renaming.NewUsername));
                    _server.Log($"User \"{sender.Username}\" renamed to \"{renaming.NewUsername}\".");
                    sender.Username = renaming.NewUsername;
                }
            }
            else
            {
                throw new NotImplementedException("No handler defined for this type of packet!");
            }
        }
    }
}
