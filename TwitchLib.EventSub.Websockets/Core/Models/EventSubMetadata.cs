using System;

namespace TwitchLib.EventSub.Websockets.Core.Models;

public class EventSubMetadata
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
}