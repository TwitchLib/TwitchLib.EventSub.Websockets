using TwitchLib.EventSub.Core.SubscriptionTypes.Automod;
using TwitchLib.EventSub.Websockets.Core.EventArgs.Automod;
using TwitchLib.EventSub.Websockets.Core.Models;

namespace TwitchLib.EventSub.Websockets.Handler.Conduit;

/// <summary>
/// Handler for 'automod.message.update' notifications
/// </summary>
class AutomodMessageUpdateV2Handler : NotificationHandler<AutomodMessageUpdateV2Args, EventSubNotification<AutomodMessageUpdateV2>>
{
    /// <inheritdoc />
    public override string SubscriptionType => "automod.message.update";

    /// <inheritdoc />
    public override string SubscriptionVersion => "2";

    /// <inheritdoc />
    public override string EventName => nameof(EventSubWebsocketClient.AutomodMessageUpdateV2);
}
