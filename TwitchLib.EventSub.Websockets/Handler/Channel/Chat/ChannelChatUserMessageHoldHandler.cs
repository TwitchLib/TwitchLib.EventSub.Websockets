using TwitchLib.EventSub.Core.SubscriptionTypes.Channel;
using TwitchLib.EventSub.Websockets.Core.EventArgs.Channel;
using TwitchLib.EventSub.Websockets.Core.Models;

namespace TwitchLib.EventSub.Websockets.Handler.Channel.Chat;

internal class ChannelChatUserMessageHoldHandler : NotificationHandler<ChannelChatUserMessageHoldArgs, EventSubNotification<ChannelChatUserMessageHold>>
{
    public override string SubscriptionType => "channel.chat.user_message_hold";

    public override string SubscriptionVersion => "1";

    public override string EventName => nameof(EventSubWebsocketClient.ChannelChatUserMessageHold);
}
