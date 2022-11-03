using System;

namespace TwitchLib.EventSub.Websockets.Core.Models
{
    public class EventSubWebsocketSessionInfo
    {
        public string Id { get; set; }
        public string Status { get; set; }
        public string DisconnectReason { get; set; }
        public int? KeepaliveTimeoutSeconds { get; set; }
        public string ReconnectUrl { get; set; }
        public DateTime ConnectedAt { get; set; }
        public DateTime? DisconnectedAt { get; set; }
        public DateTime? ReconnectingAt { get; set; }
    }
}