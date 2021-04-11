using System;

namespace Packets
{
    public enum RejectionReason { ServerFull, UsernameAlreadyUsed, UsernameUnavailable }

    // Server sends it to just connected clients

    [Packet(4, typeof(ConnectionResponsePacket)), Serializable]
    public class ConnectionResponsePacket : Packet
    {
        /// <summary>It's true if connection is confirmed, or false if connection is rejected and the client will be disconnected.</summary>
        public bool ConnectionConfirmed { get; set; }

        /// <summary>It's null if connection is confirmed.</summary>
        public RejectionReason RejectionReason { get; set; }

        /// <summary>It's null if connection is rejected.</summary>
        public string UserID { get; set; }

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
