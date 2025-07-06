using TwitchLib.EventSub.Core.SubscriptionTypes.Automod;
using TwitchLib.EventSub.Websockets.Core.EventArgs.Automod;
using TwitchLib.EventSub.Websockets.Core.Models;

namespace TwitchLib.EventSub.Websockets.Handler.Conduit;

/// <summary>
/// Handler for 'automod.settings.update' notifications
/// </summary>
class AutomodSettingsUpdateHandler : NotificationHandler<AutomodSettingsUpdateArgs, EventSubNotification<AutomodSettingsUpdate>>
{
    /// <inheritdoc />
    public override string SubscriptionType => "automod.settings.update";

    /// <inheritdoc />
    public override string SubscriptionVersion => "1";

    /// <inheritdoc />
    public override string EventName => nameof(EventSubWebsocketClient.AutomodSettingsUpdate);
}
