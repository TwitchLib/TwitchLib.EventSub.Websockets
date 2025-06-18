﻿using System;
using System.Text.Json;
using TwitchLib.EventSub.Core.SubscriptionTypes.Channel;
using TwitchLib.EventSub.Websockets.Core.EventArgs;
using TwitchLib.EventSub.Websockets.Core.EventArgs.Channel;
using TwitchLib.EventSub.Websockets.Core.Handler;
using TwitchLib.EventSub.Websockets.Core.Models;

namespace TwitchLib.EventSub.Websockets.Handler.Channel.Chat
{
    /// <summary>
    /// Handler for 'channel.chat.message_delete' notifications
    /// </summary>
    public class ChatMessageDeleteHandler : INotificationHandler
    {
        /// <inheritdoc />
        public string SubscriptionType => "channel.chat.message_delete";

        /// <inheritdoc />
        public string SubscriptionVersion => "1";

        /// <inheritdoc />
        public void Handle(EventSubWebsocketClient client, string jsonString, JsonSerializerOptions serializerOptions)
        {
            try
            {
                var data = JsonSerializer.Deserialize<EventSubNotification<ChannelChatMessageDelete>>(jsonString.AsSpan(), serializerOptions);
                if (data is null)
                    throw new InvalidOperationException("Parsed JSON cannot be null!");
                client.RaiseEvent("ChannelChatMessageDelete", new ChannelChatMessageDeleteArgs { Notification = data });
            }
            catch (Exception ex)
            {
                client.RaiseEvent("ErrorOccurred", new ErrorOccuredArgs { Exception = ex, Message = $"Error encountered while trying to handle {SubscriptionType} notification! Raw Json: {jsonString}" });
            }
        }
    }
}
