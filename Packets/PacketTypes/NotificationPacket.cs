using System;

namespace Packets
{
    public enum NotificationCode
    {
        // Notifications from a server
        ServerStopping,
        // Notifications from a client
        ClientDisconnecting,
    }

    [Packet(2, typeof(NotificationPacket)), Serializable] 
    public class NotificationPacket : Packet
    {
        public NotificationCode Code { get; set; }

        public NotificationPacket() { }

        public NotificationPacket(NotificationCode code)
        {
            Code = code;
        }
    }
}
