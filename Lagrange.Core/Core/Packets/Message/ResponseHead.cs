using Lagrange.Core.Core.Packets.Message.Routing;
using ProtoBuf;

namespace Lagrange.Core.Core.Packets.Message;

[ProtoContract]
internal class ResponseHead
{
    [ProtoMember(1)] public uint FromUin { get; set; }
    
    [ProtoMember(2)] public string? FromUid { get; set; }
    
    [ProtoMember(3)] public uint Type { get; set; }
    
    [ProtoMember(4)] public uint SigMap { get; set; } // 鬼知道是啥
    
    [ProtoMember(5)] public uint ToUin { get; set; }
    
    [ProtoMember(6)] public string? ToUid { get; set; }
    
    [ProtoMember(8)] public ResponseGrp? Grp { get; set; }
}