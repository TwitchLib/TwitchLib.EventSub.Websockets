﻿using System;
using System.Text.Json;
using TwitchLib.EventSub.Core.SubscriptionTypes.Channel;
using TwitchLib.EventSub.Websockets.Core.EventArgs;
using TwitchLib.EventSub.Websockets.Core.EventArgs.Channel;
using TwitchLib.EventSub.Websockets.Core.Handler;
using TwitchLib.EventSub.Websockets.Core.Models;

namespace TwitchLib.EventSub.Websockets.Handler.Channel.ChannelPoints.Redemptions;

/// <summary>
/// Handler for 'channel.channel_points_automatic_reward_redemption.add' notifications
/// </summary>
public class ChannelPointsAutomaticRewardRedemptionAddHandler : INotificationHandler
{
    /// <inheritdoc />
    public string SubscriptionType => "channel.channel_points_automatic_reward_redemption.add";

    /// <inheritdoc />
    public string SubscriptionVersion => "1";

    /// <inheritdoc />
    public void Handle(EventSubWebsocketClient client, string jsonString, JsonSerializerOptions serializerOptions)
    {
        try
        {
            var data = JsonSerializer.Deserialize<EventSubNotification<ChannelPointsAutomaticRewardRedemption>>(jsonString.AsSpan(), serializerOptions);

            if (data is null)
                throw new InvalidOperationException("Parsed JSON cannot be null!");

            client.RaiseEvent("ChannelPointsAutomaticRewardRedemptionAdd", new ChannelPointsAutomaticRewardRedemptionArgs { Notification = data });
        }
        catch (Exception ex)
        {
            client.RaiseEvent("ErrorOccurred", new ErrorOccuredArgs { Exception = ex, Message = $"Error encountered while trying to handle {SubscriptionType} notification! Raw Json: {jsonString}" });
        }
    }
}
