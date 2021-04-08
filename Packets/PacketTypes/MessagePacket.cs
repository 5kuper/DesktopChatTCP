using System;

namespace Packets
{
    [Packet(1, typeof(MessagePacket)), Serializable] 
    public partial class MessagePacket : Packet
    {
        public string Sender { get; set; }
        public string Content { get; set; }

        public MessagePacket() { }

        /// <summary>Clients specify their ID as sender, and the server specifies the client's username as sender.</summary>
        public MessagePacket(string sender, string content)
        {
            Sender = sender;
            Content = content;
        }

        public override void InvokeHandler()
        {
            Handle();
        }

        partial void Handle(); // The server project has one implementation, and the client project is different
    }
}
