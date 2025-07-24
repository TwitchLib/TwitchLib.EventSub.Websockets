using System;
using System.Diagnostics.CodeAnalysis;
using TwitchLib.EventSub.Core.Models;

namespace TwitchLib.EventSub.Websockets.Core.Models;

public class WebsocketEventSubMetadata : EventSubMetadata
{
    /// <summary>
    /// An ID that uniquely identifies message. 
    /// </summary>
    public string MessageId { get; set; }

    /// <summary>
    /// The type of notification.
    /// </summary>
    public string MessageType { get; set; }

    /// <summary>
    /// The UTC date and time that Twitch sent the notification.
    /// </summary>
    public DateTime MessageTimestamp { get; set; }

    /// <summary>
    /// The subscription type.
    /// </summary>
    public string? SubscriptionType { get; set; }

    /// <summary>
    /// The subscription version.
    /// </summary>
    public string? SubscriptionVersion { get; set; }

#if NET8_0_OR_GREATER
    [MemberNotNullWhen(true, nameof(SubscriptionType), nameof(SubscriptionVersion))]
#endif
    public bool HasSubscriptionInfo => SubscriptionType is not null && SubscriptionVersion is not null;
}
