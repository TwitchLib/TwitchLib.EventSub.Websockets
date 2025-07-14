using System.Text.Json;
using TwitchLib.EventSub.Websockets.Core.Models;

namespace TwitchLib.EventSub.Websockets.Core.EventArgs;

public class UnknownEventSubEventArgs : TwitchLibEventSubEventArgs<EventSubNotification<JsonElement>>
{ }
