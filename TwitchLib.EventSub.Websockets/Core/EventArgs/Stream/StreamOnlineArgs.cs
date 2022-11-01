using TwitchLib.EventSub.Core.SubscriptionTypes.Stream;
using TwitchLib.EventSub.Websockets.Core.Models;

namespace TwitchLib.EventSub.Websockets.Core.EventArgs.Stream
{
    public class StreamOnlineArgs : TwitchLibEventSubEventArgs<EventSubNotification<StreamOnline>>
    { }
}