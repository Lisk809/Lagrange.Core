using Lagrange.Core.Common.Entity;
using Lagrange.Core.Core.Context.Attributes;
using Lagrange.Core.Core.Event.Protocol.System;
using Lagrange.Core.Core.Event.Protocol.Message;
using Lagrange.Core.Message;
using System.Text;
using Lagrange.Core.Core.Event.Protocol.Action;

namespace Lagrange.Core.Core.Context.Logic.Implementation;

[BusinessLogic("OperationLogic", "Manage the user operation of the bot")]
internal class OperationLogic : LogicBase
{
    private const string Tag = nameof(OperationLogic);
    
    internal OperationLogic(ContextCollection collection) : base(collection) { }

    public async Task<List<string>> GetCookies(List<string> domains)
    {
        var fetchCookieEvent = FetchCookieEvent.Create(domains);
        var events = await Collection.Business.SendEvent(fetchCookieEvent);
        return events.Count != 0 ? ((FetchCookieEvent)events[0]).Cookies : new List<string>();
    }

    public async Task<List<BotFriend>> FetchFriends()
    {
        var fetchFriendsEvent = FetchFriendsEvent.Create();
        var events = await Collection.Business.SendEvent(fetchFriendsEvent);
        return events.Count != 0 ? ((FetchFriendsEvent)events[0]).Friends : new List<BotFriend>();
    }
    
    public async Task<List<BotGroupMember>> FetchMembers(uint groupUin)
    {
        var fetchFriendsEvent = FetchMembersEvent.Create(groupUin);
        var events = await Collection.Business.SendEvent(fetchFriendsEvent);
        return events.Count != 0 ? ((FetchMembersEvent)events[0]).Members : new List<BotGroupMember>();
    }

    public async Task<MessageResult> SendMessage(MessageChain chain)
    {
        var sendMessageEvent = SendMessageEvent.Create(chain);
        var events = await Collection.Business.SendEvent(sendMessageEvent);
        return ((SendMessageEvent)events[0]).MsgResult;
    }

    public async Task<bool> MuteGroupMember(uint groupUin, uint targetUin, int duration)
    {
        string? uid = await Collection.Business.CachingLogic.ResolveUid(groupUin, targetUin);
        if (uid == null) return false;
        
        var muteGroupMemberEvent = GroupMuteMemberEvent.Create(groupUin, duration, uid);
        var events = await Collection.Business.SendEvent(muteGroupMemberEvent);
        return events.Count != 0 && ((GroupMuteMemberEvent)events[0]).ResultCode == 0;
    }
    
    public async Task<bool> MuteGroupGlobal(uint groupUin, bool isMute)
    {
        var muteGroupMemberEvent = GroupMuteGlobalEvent.Create(groupUin, isMute);
        var events = await Collection.Business.SendEvent(muteGroupMemberEvent);
        return events.Count != 0 && ((GroupMuteGlobalEvent)events[0]).ResultCode == 0;
    }
    
    public async Task<bool> KickGroupMember(uint groupUin, uint targetUin, bool rejectAddRequest)
    {
        string? uid = await Collection.Business.CachingLogic.ResolveUid(groupUin, targetUin);
        if (uid == null) return false;
        
        var muteGroupMemberEvent = GroupKickMemberEvent.Create(groupUin, uid, rejectAddRequest);
        var events = await Collection.Business.SendEvent(muteGroupMemberEvent);
        return events.Count != 0 && ((GroupKickMemberEvent)events[0]).ResultCode == 0;
    }
    
    public async Task<bool> SetGroupAdmin(uint groupUin, uint targetUin, bool isAdmin)
    {
        string? uid = await Collection.Business.CachingLogic.ResolveUid(groupUin, targetUin);
        if (uid == null) return false;
        
        var muteGroupMemberEvent = GroupSetAdminEvent.Create(groupUin, uid, isAdmin);
        var events = await Collection.Business.SendEvent(muteGroupMemberEvent);
        return events.Count != 0 && ((GroupSetAdminEvent)events[0]).ResultCode == 0;
    }

    public async Task<bool> GetHighwayAddress()
    {
        var highwayUrlEvent = HighwayUrlEvent.Create();
        var events = await Collection.Business.SendEvent(highwayUrlEvent);
        return events.Count != 0 && ((HighwayUrlEvent)events[0]).ResultCode == 0;
    }

    public async Task<bool> RecallGroupMessage(uint groupUin, MessageResult result)
    {
        if (result.Sequence == null) return false;
        
        var recallMessageEvent = RecallGroupMessageEvent.Create(groupUin, (uint)result.Sequence);
        var events = await Collection.Business.SendEvent(recallMessageEvent);
        return events.Count != 0 && ((RecallGroupMessageEvent)events[0]).ResultCode == 0;
    }

    private static int CalculateBkn(string sKey) => 
        (int)sKey.Aggregate<char, long>(5381, (current, t) => current + (current << 5) + t) & int.MaxValue;
    
    public int GetCsrfToken() => CalculateBkn(Encoding.ASCII.GetString(Collection.Keystore.Session.D2Key));
}