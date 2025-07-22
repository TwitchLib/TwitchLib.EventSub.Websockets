using TwitchLib.EventSub.Core.SubscriptionTypes.Channel;
using TwitchLib.EventSub.Websockets;
using TwitchLib.EventSub.Websockets.Core.EventArgs.Channel;
using TwitchLib.EventSub.Websockets.Core.Models;
using TwitchLib.EventSub.Websockets.Handler;

internal class ChannelModerateHandler : NotificationHandler<ChannelModerateArgs, EventSubNotification<ChannelModerate>>
{
    public override string SubscriptionType => "channel.moderate";

    public override string SubscriptionVersion => "1";

    public override string EventName => nameof(EventSubWebsocketClient.ChannelModerate);
}
