using TwitchLib.EventSub.Core.SubscriptionTypes.Channel;
using TwitchLib.EventSub.Websockets.Core.Models;

namespace TwitchLib.EventSub.Websockets.Core.EventArgs.Channel;

/// <summary>
/// <see href="https://dev.twitch.tv/docs/eventsub/eventsub-subscription-types/#channelmoderate-v2">Twitch Documentation</see><br/>
/// When a moderator performs a moderation action in a channel.<br/>
/// Required Scopes: Check documenation for full list<br/>
/// </summary>
public class ChannelModerateArgs : TwitchLibEventSubEventArgs<EventSubNotification<ChannelModerate>>
{ }