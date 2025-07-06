using TwitchLib.EventSub.Core.SubscriptionTypes.Automod;
using TwitchLib.EventSub.Websockets.Core.Models;

namespace TwitchLib.EventSub.Websockets.Core.EventArgs.Automod;

public class AutomodTermsUpdateArgs : TwitchLibEventSubEventArgs<EventSubNotification<AutomodTermsUpdate>>
{ }
