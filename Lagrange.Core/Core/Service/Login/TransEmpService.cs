using Lagrange.Core.Common;
using Lagrange.Core.Core.Event.Protocol;
using Lagrange.Core.Core.Event.Protocol.Login;
using Lagrange.Core.Core.Packets;
using Lagrange.Core.Core.Packets.Login.WtLogin.Entity;
using Lagrange.Core.Core.Packets.Tlv;
using Lagrange.Core.Core.Service.Abstraction;
using Lagrange.Core.Utility.Binary;

namespace Lagrange.Core.Core.Service.Login;

[EventSubscribe(typeof(TransEmpEvent))]
[Service("wtlogin.trans_emp")]
internal class TransEmpService : BaseService<TransEmpEvent>
{
    protected override bool Parse(SsoPacket input, BotKeystore keystore, BotAppInfo appInfo, BotDeviceInfo device, 
        out TransEmpEvent output, out List<ProtocolEvent>? extraEvents)
    {
        var payload = input.Payload;
        var packet = TransEmp.DeserializeBody(keystore, payload, out ushort command);
        
        if (command == 0x31)
        {
            var tlvs = TransEmp31.Deserialize(packet, keystore, out var signature);

            var qrCode = ((TlvQrCode17)tlvs[0x17]).QrCode;
            uint expiration = ((TlvQrCode1C)tlvs[0x01C]).ExpireSec;
            string url = ((TlvQrCodeD1Resp)tlvs[0x0D1]).Url;
            string qrSig = ((TlvQrCodeD1Resp)tlvs[0x0D1]).QrSig;

            output = TransEmpEvent.Result(qrCode, expiration, url, qrSig, signature);
        }
        else
        {
            var tlvs = TransEmp12.Deserialize(packet, out var state);

            if (state == TransEmp12.State.Confirmed)
            {
                var tgtgtKey = ((TlvQrCode1E)tlvs[0x1E]).TgtgtKey;
                var tempPassword = ((TlvQrCode18)tlvs[0x18]).TempPassword;
                var noPicSig = ((TlvQrCode19)tlvs[0x19]).NoPicSig;

                output = TransEmpEvent.Result(state, tgtgtKey, tempPassword, noPicSig);
            }
            else
            {
                output = TransEmpEvent.Result(state, null, null, null);
            }
        }

        extraEvents = null;
        return true;
    }
    
    protected override bool Build(TransEmpEvent input, BotKeystore keystore, BotAppInfo appInfo, BotDeviceInfo device, 
        out BinaryPacket output, out List<BinaryPacket>? extraPackets)
    {
        output = input.EventState == TransEmpEvent.State.FetchQrCode
            ? new TransEmp31(keystore, appInfo, device).ConstructPacket()
            : new TransEmp12(keystore, appInfo, device).ConstructPacket();
        
        extraPackets = null;
        return true;
    }
}