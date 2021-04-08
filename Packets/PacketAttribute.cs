using System;

namespace Packets
{
    [AttributeUsage(AttributeTargets.Class)]
    internal class PacketAttribute : Attribute
    {
        /// <summary>Allows you to serialize and deserialize the class using Packet methods.</summary>
        /// <param name="id">A unique ID that cannot be zero.</param>
        /// <param name="type">A type of the class.</param>
        /// <exception cref="ArgumentException"></exception>
        public PacketAttribute(byte id, Type type)
        {
            if (id == 0)
            {
                throw new ArgumentException("The ID cannot be zero!");
            }
            if (type.BaseType != typeof(Packet))
            {
                throw new ArgumentException("The type must derive from the Packet class!");
            }
            if (!Packet.PacketTypes.ContainsValue(type))
            {
                Packet.PacketTypes.Add(id, type);
            }
        }
    }
}
