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
                _server.BroadcastPacket(message, sender);
            }
            else if (packet is WarningPacket warning)
            {
                switch (warning.Code)
                {
                    case WarningCode.ClientDisconnecting:
                        sender.Close();
                        _server.Log($"User \"{sender.Username}\" disconnected!");
                        break;
                    default:
                        _server.Log($"User \"{sender.Username}\" sent invalid warning!");
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
                }
            }
            else
            {
                throw new NotImplementedException("No handler defined for this type of packet!");
            }
        }
    }
}
