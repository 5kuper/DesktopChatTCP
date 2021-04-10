using System;

namespace Packets
{
    public enum RejectionReason { ServerFull, UsernameAlreadyUsed, UsernameUnavailable }

    [Packet(4, typeof(ConnectionResponsePacket)), Serializable]
    public class ConnectionResponsePacket : Packet
    {
        public bool ConnectionConfirmed { get; set; }

        public string UserID { get; set; }

        public RejectionReason RejectionReason { get; set; }

        public ConnectionResponsePacket() { }

        public ConnectionResponsePacket(string userID)
        {
            ConnectionConfirmed = true;
            UserID = userID;
        }

        public ConnectionResponsePacket(RejectionReason reason)
        {
            ConnectionConfirmed = false;
            RejectionReason = reason;
        }
    }
}
