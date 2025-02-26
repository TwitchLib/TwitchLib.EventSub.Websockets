using System;
using System.Text.Json;
using TwitchLib.EventSub.Core.SubscriptionTypes.Channel;
using TwitchLib.EventSub.Websockets.Core.EventArgs;
using TwitchLib.EventSub.Websockets.Core.EventArgs.Channel;
using TwitchLib.EventSub.Websockets.Core.Handler;
using TwitchLib.EventSub.Websockets.Core.Models;

namespace TwitchLib.EventSub.Websockets.Handler.Channel.SharedChat
{
    /// <summary>
    /// Handler for 'channel.shared_chat.begin' notifications
    /// </summary>
    public class ChannelSharedChatSessionBeginHandler : INotificationHandler
    {
        /// <inheritdoc />
        public string SubscriptionType => "channel.shared_chat.begin";
        
        /// <inheritdoc />
        public void Handle(EventSubWebsocketClient client, string jsonString, JsonSerializerOptions serializerOptions)
        {
            try
            {
                var data = JsonSerializer.Deserialize<EventSubNotification<ChannelSharedChatSessionBegin>>(jsonString.AsSpan(), serializerOptions);

                if (data is null)
                    throw new InvalidOperationException("Parsed JSON cannot be null!");

                client.RaiseEvent("ChannelSharedChatSessionBegin", new ChannelSharedChatSessionBeginArgs { Notification = data });
            }
            catch (Exception ex)
            {
                client.RaiseEvent("ErrorOccurred", new ErrorOccuredArgs { Exception = ex, Message = $"Error encountered while trying to handle channel.shared_chat.begin notification! Raw Json: {jsonString}" });
            }
        }
    }
}