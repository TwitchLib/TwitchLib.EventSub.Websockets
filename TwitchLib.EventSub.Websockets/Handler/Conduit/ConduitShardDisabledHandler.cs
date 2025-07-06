using TwitchLib.EventSub.Core.SubscriptionTypes.Conduit;
using TwitchLib.EventSub.Websockets.Core.EventArgs.Conduit;
using TwitchLib.EventSub.Websockets.Core.Models;

namespace TwitchLib.EventSub.Websockets.Handler.Conduit;

/// <summary>
/// Handler for 'conduit.shard.disabled' notifications
/// </summary>
class ConduitShardDisabledHandler : NotificationHandler<ConduitShardDisabledArgs, EventSubNotification<ConduitShardDisabled>>
{
    /// <inheritdoc />
    public override string SubscriptionType => "conduit.shard.disabled";

    /// <inheritdoc />
    public override string SubscriptionVersion => "1";

    /// <inheritdoc />
    public override string EventName => nameof(EventSubWebsocketClient.ConduitShardDisabled);
}
