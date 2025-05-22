using System;

namespace TwitchLib.EventSub.Websockets.Core.Models;

public class EventSubWebsocketSessionInfo
{
    /// <summary>
    /// An ID that uniquely identifies this WebSocket connection.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// The connection’s status.
    /// </summary>
    public string Status { get; set; }
    public string DisconnectReason { get; set; }

    /// <summary>
    /// The maximum number of seconds that you should expect silence before receiving a keepalive message.
    /// </summary>
    public int? KeepaliveTimeoutSeconds { get; set; }

    /// <summary>
    /// The URL to reconnect to. 
    /// </summary>
    public string? ReconnectUrl { get; set; }

    /// <summary>
    /// The UTC date and time when the connection was created.
    /// </summary>
    public DateTime ConnectedAt { get; set; }
    public DateTime? DisconnectedAt { get; set; }
    public DateTime? ReconnectingAt { get; set; }
}