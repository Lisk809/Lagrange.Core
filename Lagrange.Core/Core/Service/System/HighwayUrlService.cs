using Lagrange.Core.Common;
using Lagrange.Core.Core.Event.Protocol;
using Lagrange.Core.Core.Event.Protocol.System;
using Lagrange.Core.Core.Packets;
using Lagrange.Core.Core.Packets.Action.HttpConn;
using Lagrange.Core.Core.Service.Abstraction;
using Lagrange.Core.Utility.Binary;
using Lagrange.Core.Utility.Extension;
using ProtoBuf;
using BitConverter = Lagrange.Core.Utility.Binary.BitConverter;

namespace Lagrange.Core.Core.Service.System;

[EventSubscribe(typeof(HighwayUrlEvent))]
[Service("HttpConn.0x6ff_501")]
internal class HighwayUrlService : BaseService<HighwayUrlEvent>
{
    protected override bool Build(HighwayUrlEvent input, BotKeystore keystore, BotAppInfo appInfo, BotDeviceInfo device,
        out BinaryPacket output, out List<BinaryPacket>? extraPackets)
    {
        var packet = new HttpConn0x6ff_501
        {
            HttpConn = new HttpConn
            {
                Field1 = 0,
                Field2 = 0,
                Field3 = 16,
                Field4 = 1,
                Tgt = keystore.Session.Tgt.Hex().ToLower(),
                Field6 = 3,
                ServiceTypes = new List<int> { 1, 5, 10, 21 },
                Field9 = 2,
                Field10 = 9,
                Field11 = 8,
                Ver = "1.0.1"
            }
        };
        
        using var stream = new MemoryStream();
        Serializer.Serialize(stream, packet);
        output = new BinaryPacket(stream);
        
        extraPackets = null;
        return true;
    }

    protected override bool Parse(SsoPacket input, BotKeystore keystore, BotAppInfo appInfo, BotDeviceInfo device,
        out HighwayUrlEvent output, out List<ProtocolEvent>? extraEvents)
    {
        var payload = input.Payload.ReadBytes(BinaryPacket.Prefix.Uint32 | BinaryPacket.Prefix.WithPrefix);
        var packet = Serializer.Deserialize<HttpConn0x6ff_501Response>(payload.AsSpan());
        
        var servers = new Dictionary<uint, List<Uri>>();
        foreach (var serverInfo in packet.HttpConn.ServerInfos)
        {
            uint type = serverInfo.ServiceType;
            servers[type] = new List<Uri>();
            
            foreach (var serverAddr in serverInfo.ServerAddrs)
            {
                var ip = BitConverter.GetBytes(serverAddr.Ip);
                servers[type].Add(new Uri($"https://{ip[0]}.{ip[1]}.{ip[2]}.{ip[3]}:{serverAddr.Port}"));
            }
        }
        
        output = HighwayUrlEvent.Result(0, servers);
        extraEvents = null;
        return true;
    }
}