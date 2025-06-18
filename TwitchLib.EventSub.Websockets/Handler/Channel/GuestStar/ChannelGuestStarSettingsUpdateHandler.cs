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
    public class ChannelGuestStarSettingsUpdateHandler : INotificationHandler
    {
        /// <inheritdoc />
        public string SubscriptionType => "channel.guest_star_settings.update";

        /// <inheritdoc />
        public string SubscriptionVersion => "beta";

        /// <inheritdoc />
        public void Handle(EventSubWebsocketClient client, string jsonString, JsonSerializerOptions serializerOptions)
        {
            try
            {
                var data = JsonSerializer.Deserialize<EventSubNotification<ChannelGuestStarSettingsUpdate>>(jsonString.AsSpan(), serializerOptions);

                if (data is null)
                    throw new InvalidOperationException("Parsed JSON cannot be null!");

                client.RaiseEvent("ChannelGuestStarSettingsUpdate", new ChannelGuestStarSettingsUpdateArgs { Notification = data });
            }
            catch (Exception ex)
            {
                client.RaiseEvent("ErrorOccurred", new ErrorOccuredArgs { Exception = ex, Message = $"Error encountered while trying to handle channel.guest_star_settings.update notification! Raw Json: {jsonString}" });
            }
        }
    }
}