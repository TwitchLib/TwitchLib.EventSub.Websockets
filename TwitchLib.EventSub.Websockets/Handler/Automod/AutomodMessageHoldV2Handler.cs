using TwitchLib.EventSub.Core.SubscriptionTypes.Automod;
using TwitchLib.EventSub.Websockets.Core.EventArgs.Automod;
using TwitchLib.EventSub.Websockets.Core.Models;

namespace TwitchLib.EventSub.Websockets.Handler.Conduit;

/// <summary>
/// Handler for 'automod.message.hold' notifications
/// </summary>
class AutomodMessageHoldV2Handler : NotificationHandler<AutomodMessageHoldV2Args, EventSubNotification<AutomodMessageHoldV2>>
{
    /// <inheritdoc />
    public override string SubscriptionType => "automod.message.hold";

    /// <inheritdoc />
    public override string SubscriptionVersion => "2";

    /// <inheritdoc />
    public override string EventName => nameof(EventSubWebsocketClient.AutomodMessageHoldV2);
}
