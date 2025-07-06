using TwitchLib.EventSub.Core.SubscriptionTypes.Automod;
using TwitchLib.EventSub.Websockets.Core.EventArgs.Automod;
using TwitchLib.EventSub.Websockets.Core.Models;

namespace TwitchLib.EventSub.Websockets.Handler.Conduit;

/// <summary>
/// Handler for 'automod.terms.update' notifications
/// </summary>
class AutomodTermsUpdateHandler : NotificationHandler<AutomodTermsUpdateArgs, EventSubNotification<AutomodTermsUpdate>>
{
    /// <inheritdoc />
    public override string SubscriptionType => "automod.terms.update";

    /// <inheritdoc />
    public override string SubscriptionVersion => "1";

    /// <inheritdoc />
    public override string EventName => nameof(EventSubWebsocketClient.AutomodTermsUpdate);
}
