using TwitchLib.EventSub.Core.SubscriptionTypes.Channel;
using TwitchLib.EventSub.Websockets.Core.EventArgs.Channel;
using TwitchLib.EventSub.Websockets.Core.Models;

namespace TwitchLib.EventSub.Websockets.Handler.Channel.Chat;

internal class ChannelChatSettingsUpdateHandler : NotificationHandler<ChannelChatSettingsUpdateArgs, EventSubNotification<ChannelChatSettingsUpdate>>
{
    public override string SubscriptionType => "channel.chat_settings.update";

    public override string SubscriptionVersion => "1";

    public override string EventName => nameof(EventSubWebsocketClient.ChannelChatSettingsUpdate);
}
