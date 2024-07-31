﻿using System;
using System.Text.Json;
using TwitchLib.EventSub.Core.SubscriptionTypes.Channel;
using TwitchLib.EventSub.Websockets.Core.EventArgs;
using TwitchLib.EventSub.Websockets.Core.EventArgs.Channel;
using TwitchLib.EventSub.Websockets.Core.Handler;
using TwitchLib.EventSub.Websockets.Core.Models;

namespace TwitchLib.EventSub.Websockets.Handler.Channel.Moderation;

/// <summary>
/// Handler for 'channel.moderate' notifications
/// </summary>
public class ChannelModerateHandler : INotificationHandler
{
	/// <inheritdoc />
	public string SubscriptionType => "channel.moderate";

	/// <inheritdoc />
	public void Handle(EventSubWebsocketClient client, string jsonString, JsonSerializerOptions serializerOptions)
	{
		try
		{
			var data = JsonSerializer.Deserialize<EventSubNotification<ChannelModerate>>(jsonString.AsSpan(), serializerOptions);

			if (data is null)
				throw new InvalidOperationException("Parsed JSON cannot be null!");

			client.RaiseEvent("ChannelModerate", new ChannelModerateArgs { Notification = data });
		}
		catch (Exception ex)
		{
			client.RaiseEvent("ErrorOccurred", new ErrorOccuredArgs { Exception = ex, Message = $"Error encountered while trying to handle {SubscriptionType} notification! Raw Json: {jsonString}" });
		}
	}
}