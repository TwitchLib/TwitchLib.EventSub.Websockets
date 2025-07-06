using TwitchLib.EventSub.Core.SubscriptionTypes.Automod;
using TwitchLib.EventSub.Websockets.Core.EventArgs.Automod;
using TwitchLib.EventSub.Websockets.Core.Models;

namespace TwitchLib.EventSub.Websockets.Handler.Conduit;

/// <summary>
/// Handler for 'automod.message.update' notifications
/// </summary>
class AutomodMessageUpdateHandler : NotificationHandler<AutomodMessageUpdateArgs, EventSubNotification<AutomodMessageUpdate>>
{
    /// <inheritdoc />
    public override string SubscriptionType => "automod.message.update";

    /// <inheritdoc />
    public override string SubscriptionVersion => "1";

    /// <inheritdoc />
    public override string EventName => nameof(EventSubWebsocketClient.AutomodMessageUpdate);
}
