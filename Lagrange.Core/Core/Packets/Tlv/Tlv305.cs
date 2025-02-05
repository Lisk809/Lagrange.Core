using Lagrange.Core.Utility.Binary;
using Lagrange.Core.Utility.Binary.Tlv;
using Lagrange.Core.Utility.Binary.Tlv.Attributes;

#pragma warning disable CS8618

namespace Lagrange.Core.Core.Packets.Tlv;

[Tlv(0X305)]
internal class Tlv305 : TlvBody
{
    [BinaryProperty(Prefix.None)] public byte[] D2Key { get; set; }
}