using System;

namespace Packets
{
    // Response to RenamingRequestPacket from server to client

    [Packet(6, typeof(RenamingResponsePacket)), Serializable]
    public class RenamingResponsePacket : Packet
    {
        public bool RenamingConfirmed { get; set; }

        /// <summary>It's null if renaming is confirmed.</summary>
        public RejectionReason RejectionReason { get; set; }

        /// <summary>It's null if renaming is rejected.</summary>
        public string NewUsername { get; set; }

        public RenamingResponsePacket() { }

        public RenamingResponsePacket(string username)
        {
            RenamingConfirmed = true;
            NewUsername = username;
        }

        public RenamingResponsePacket(RejectionReason reason)
        {
            RenamingConfirmed = false;
            RejectionReason = reason;
        }
    }
}
