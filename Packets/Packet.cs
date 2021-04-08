using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;

namespace Packets
{
    // For create a new packet type you need to inherit your class from the Packet and
    // add to it SerializableAttribute and PacketAttribute in which specify a unique ID and the type itself.

    public abstract class Packet
    {
        /// <summary>Packet IDs & types specified in PacketAttributes.</summary>
        public static Dictionary<byte, Type> PacketTypes = new Dictionary<byte, Type>();

        /// <summary>Calls the PacketAttribute constructor for each derived class.</summary>
        static Packet()
        {
            var subclassTypes = Assembly
                .GetAssembly(typeof(Packet))
                ?.GetTypes()
                .Where(t => t.IsSubclassOf(typeof(Packet)));

            if (subclassTypes != null)
                foreach (Type subclass in subclassTypes)
                {
                    Attribute.GetCustomAttribute(subclass, typeof(PacketAttribute));
                }
        }

        /// <summary>Deserializes the data to a packet.</summary>
        /// <returns>The Packet being deserialized.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static Packet Deserialize(byte[] data)
        {
            try
            {
                Type type = PacketTypes[data[0]];
                data = data.Skip(1).ToArray();

                XmlSerializer serializer = new XmlSerializer(type);
                using (MemoryStream stream = new MemoryStream(data))
                {
                    return (Packet)serializer.Deserialize(stream);
                }

            }
            catch
            {
                throw new ArgumentException("The data cannot be deserialized to a packet!");
            }
        }

        /// <summary>Serializes the packet to a byte array.</summary>
        /// <param name="data">The array for store serialized data.</param>
        /// <returns>The data length.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public int Serialize(out byte[] data)
        {
            List<byte> list = new List<byte> { PacketTypes.FirstOrDefault(x => x.Value == GetType()).Key };
            if (list[0] == 0)
            {
                throw new InvalidOperationException($"Packet type \"{GetType().Name}\" is not contained in Packet.PacketTypes!");
            }

            XmlSerializer serializer = new XmlSerializer(GetType());
            using (MemoryStream stream = new MemoryStream())
            {
                serializer.Serialize(stream, this);
                list.AddRange(stream.ToArray());

                data = list.ToArray();
                return data.Length;
            }
        }

        /// <summary>Invokes a method that handle the packet.</summary>
        public abstract void InvokeHandler();
    }
}
