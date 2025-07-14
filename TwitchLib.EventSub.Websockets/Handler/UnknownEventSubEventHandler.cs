using System.Text.Json;
using TwitchLib.EventSub.Websockets.Core.EventArgs;
using TwitchLib.EventSub.Websockets.Core.Models;

namespace TwitchLib.EventSub.Websockets.Handler;

class UnknownEventSubEventHandler : NotificationHandler<UnknownEventSubEventArgs, EventSubNotification<JsonElement>>
{
    /// <inheritdoc />
    public override string SubscriptionType => string.Empty;

    /// <inheritdoc />
    public override string SubscriptionVersion => string.Empty;

    /// <inheritdoc />
    public override string EventName => nameof(EventSubWebsocketClient.UnknownEventSubEvent);
}
