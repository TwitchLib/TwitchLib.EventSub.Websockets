using TwitchLib.EventSub.Core.SubscriptionTypes.Channel;
using TwitchLib.EventSub.Websockets.Core.EventArgs.Channel;
using TwitchLib.EventSub.Websockets.Core.Models;

namespace TwitchLib.EventSub.Websockets.Handler.Channel.Bits;

internal class ChannelBitsUseHandler : NotificationHandler<ChannelBitsUseArgs, EventSubNotification<ChannelBitUse>>
{
    public override string SubscriptionType => "channel.bits.use";

    public override string SubscriptionVersion => "1";

    public override string EventName => nameof(EventSubWebsocketClient.ChannelBitsUse);
}
