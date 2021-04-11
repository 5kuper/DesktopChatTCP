using System;

namespace Packets
{
    [Packet(1, typeof(MessagePacket)), Serializable] 
    public class MessagePacket : Packet
    {
        public string Sender { get; set; }
        public string Content { get; set; }

        public MessagePacket() { }

        /// <summary>Constructor for client.</summary>
        public MessagePacket(string content)
        {
            Content = content;
        }

        /// <summary>Constructor for server.</summary>
        public MessagePacket(string sender, string content)
        {
            Sender = sender;
            Content = content;
        }
    }
}
