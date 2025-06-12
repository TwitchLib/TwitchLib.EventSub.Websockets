namespace TwitchLib.EventSub.Websockets.Core.Models
{
    public class EventSubNotificationPayload<T>
    {
        public EventSubSubscription Subscription{ get; set; }

        /// <summary>
        /// The event’s data.
        /// </summary>
        public T Event { get; set; }
    }
}