namespace TwitchLib.EventSub.Websockets.Core.Models
{
    public class EventSubWebsocketSessionInfoMessage
    {
        public EventSubMetadata Metadata { get; set; }
        public EventSubWebsocketSessionInfoPayload Payload { get; set; }
    }
}