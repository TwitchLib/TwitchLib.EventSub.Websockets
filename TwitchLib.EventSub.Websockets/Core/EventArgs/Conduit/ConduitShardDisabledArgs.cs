using TwitchLib.EventSub.Core.SubscriptionTypes.Conduit;
using TwitchLib.EventSub.Websockets.Core.Models;

namespace TwitchLib.EventSub.Websockets.Core.EventArgs.Conduit;

public class ConduitShardDisabledArgs : TwitchLibEventSubEventArgs<EventSubNotification<ConduitShardDisabled>>
{ }
