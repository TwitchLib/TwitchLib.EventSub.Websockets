using System;
using System.Text.Json;
using TwitchLib.EventSub.Core.SubscriptionTypes.Channel;
using TwitchLib.EventSub.Websockets.Core.EventArgs;
using TwitchLib.EventSub.Websockets.Core.EventArgs.Channel;
using TwitchLib.EventSub.Websockets.Core.Handler;
using TwitchLib.EventSub.Websockets.Core.Models;

namespace TwitchLib.EventSub.Websockets.Handler.Channel.GuestStar
{
    /// <summary>
    /// Handler for 'channel.guest_star_slot.update' notifications
    /// </summary>
    public class ChannelGuestStarSlotUpdateHandler : INotificationHandler
    {
        /// <inheritdoc />
        public string SubscriptionType => "channel.guest_star_slot.update";
        
        /// <inheritdoc />
        public void Handle(EventSubWebsocketClient client, string jsonString, JsonSerializerOptions serializerOptions)
        {
            try
            {
                var data = JsonSerializer.Deserialize<EventSubNotification<ChannelGuestStarSlotUpdate>>(jsonString.AsSpan(), serializerOptions);

                if (data is null)
                    throw new InvalidOperationException("Parsed JSON cannot be null!");

                client.RaiseEvent("ChannelGuestStarSlotUpdate", new ChannelGuestStarSlotUpdateArgs { Notification = data });
            }
            catch (Exception ex)
            {
                client.RaiseEvent("ErrorOccurred", new ErrorOccuredArgs { Exception = ex, Message = $"Error encountered while trying to handle channel.guest_star_slot.update notification! Raw Json: {jsonString}" });
            }
        }
    }
}