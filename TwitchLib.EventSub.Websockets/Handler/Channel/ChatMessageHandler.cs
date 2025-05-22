using System;
using System.Text.Json;
using TwitchLib.EventSub.Core.SubscriptionTypes.Channel;
using TwitchLib.EventSub.Websockets.Core.EventArgs;
using TwitchLib.EventSub.Websockets.Core.EventArgs.Channel;
using TwitchLib.EventSub.Websockets.Core.Handler;
using TwitchLib.EventSub.Websockets.Core.Models;

namespace TwitchLib.EventSub.Websockets.Handler.Channel
{
    /// <summary>
    /// Handler for 'channel.chat.nessage' notifications
    /// </summary>
    public class ChatMessageHandler : INotificationHandler
    {
        /// <inheritdoc />
        public string SubscriptionType => "channel.chat.message";

        /// <inheritdoc />
        public void Handle(EventSubWebsocketClient client, string jsonString, JsonSerializerOptions serializerOptions)
        {
            try
            {
                var data = JsonSerializer.Deserialize<EventSubNotification<ChannelChatMessage>>(jsonString.AsSpan(), serializerOptions);
                if (data is null)
                    throw new InvalidOperationException("Parsed JSON cannot be null!");
                client.RaiseEvent("ChannelChatMessage", new ChannelChatMessageArgs { Notification = data });
            }
            catch (Exception ex)
            {
                client.RaiseEvent("ErrorOccurred", new ErrorOccuredArgs { Exception = ex, Message = $"Error encountered while trying to handle {SubscriptionType} notification! Raw Json: {jsonString}" });
            }
        }
    }
}
