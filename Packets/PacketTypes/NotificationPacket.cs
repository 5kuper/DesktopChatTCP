using System;
using System.Collections.Generic;

namespace Packets
{
    public enum NotificationCode
    {
        // Notifications from a server
        ServerStopping, UserConnected, UserDisconnected, UserRenamed, UserKicked,
        // Notifications from a client
        ClientDisconnecting,
    }

    [Packet(2, typeof(NotificationPacket)), Serializable] 
    public class NotificationPacket : Packet
    {
        public NotificationCode Code { get; set; }

        /// <summary>Some types of notifications use it for additional information.</summary>
        public List<string> Content { get; set; }

        public NotificationPacket() { }

        public NotificationPacket(NotificationCode code, params string[] content)
        {
            Code = code;

            if (content != null)
            {
                Content = new List<string>();
                foreach (string s in content)
                {
                    Content.Add(s);
                }
            }
        }
    }
}
