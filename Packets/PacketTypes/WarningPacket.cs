using System;

namespace Packets
{
    public enum WarningCode { }

    [Packet(2, typeof(WarningPacket)), Serializable] 
    public class WarningPacket : Packet
    {
        public int Code { get; set; }

        public WarningPacket() { }

        public WarningPacket(int code)
        {
            Code = code;
        }
    }
}
