using Lagrange.Core.Common;
using Lagrange.Core.Core.Event.Protocol;
using Lagrange.Core.Core.Event.Protocol.Login;
using Lagrange.Core.Core.Packets;
using Lagrange.Core.Core.Packets.Tlv;
using Lagrange.Core.Core.Service.Abstraction;
using Lagrange.Core.Utility.Binary;

namespace Lagrange.Core.Core.Service.Login;

[EventSubscribe(typeof(LoginEvent))]
[Service("wtlogin.login")]
internal class LoginService : BaseService<LoginEvent>
{
    protected override bool Parse(SsoPacket input, BotKeystore keystore, BotAppInfo appInfo, BotDeviceInfo device, 
        out LoginEvent output, out List<ProtocolEvent>? extraEvents)
    {
        var tlvs = Packets.Login.WtLogin.Entity.Login.Deserialize(input.Payload, keystore, out var state);

        if (state == Packets.Login.WtLogin.Entity.Login.State.Success)
        {
            var tlv11A = (Tlv11A)tlvs[0x11A];
            var tlv305 = (Tlv305)tlvs[0x305];
            var tlv543 = (Tlv543)tlvs[0x543];
            var tlv10A = (Tlv10A)tlvs[0x10A];
            var tlv143 = (Tlv143)tlvs[0x143];
            var tlv106 = (Tlv106Response)tlvs[0x106];

            keystore.Session.D2Key = tlv305.D2Key;
            keystore.Uid = tlv543.Layer1.Layer2.Uid;
            keystore.Session.Tgt = tlv10A.Tgt;
            keystore.Session.D2 = tlv143.D2;
            keystore.Session.TempPassword = tlv106.Temp;
            
            output = LoginEvent.Result((int)state, tlv11A.Age, tlv11A.Gender, tlv11A.Nickname);
            extraEvents = null;
            return true;
        }
        else
        {
            if (tlvs.TryGetValue(0x146, out var tlv))
            {
                var tlv146 = (Tlv146)tlv;
                output = LoginEvent.Result((int)state, tlv146.Tag, tlv146.Message);
                extraEvents = null;
                return true;
            }
            
            output = LoginEvent.Result((int)state);
            extraEvents = null;
            return true;
        }
    }

    protected override bool Build(LoginEvent input, BotKeystore keystore, BotAppInfo appInfo, BotDeviceInfo device, 
        out BinaryPacket output, out List<BinaryPacket>? extraPackets)
    {
        var packet = new Packets.Login.WtLogin.Entity.Login(keystore, appInfo, device);
        output = packet.ConstructPacket();
        
        extraPackets = null;
        return true;
    }
}