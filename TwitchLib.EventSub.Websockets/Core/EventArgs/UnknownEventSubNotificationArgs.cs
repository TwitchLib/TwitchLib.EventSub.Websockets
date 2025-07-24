using System.Text.Json;
using TwitchLib.EventSub.Core.EventArgs;

namespace TwitchLib.EventSub.Websockets.Core.EventArgs;

public class UnknownEventSubNotificationArgs : TwitchLibEventSubNotificationArgs<JsonElement>
{ }
