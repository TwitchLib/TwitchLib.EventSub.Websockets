using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TwitchLib.EventSub.Core;
using TwitchLib.EventSub.Core.Extensions;
using TwitchLib.EventSub.Core.SubscriptionTypes.Channel;
using TwitchLib.EventSub.Websockets.Client;
using TwitchLib.EventSub.Websockets.Core.EventArgs;
using TwitchLib.EventSub.Websockets.Core.EventArgs.Channel;
using TwitchLib.EventSub.Websockets.Core.EventArgs.Stream;
using TwitchLib.EventSub.Websockets.Core.EventArgs.User;
using TwitchLib.EventSub.Websockets.Core.Handler;
using TwitchLib.EventSub.Websockets.Core.Models;
using TwitchLib.EventSub.Websockets.Extensions;

namespace TwitchLib.EventSub.Websockets
{
    /// <summary>
    /// EventSubWebsocketClient used to subscribe to EventSub notifications via Websockets
    /// </summary>
    public class EventSubWebsocketClient
    {
        #region Events

        /// <summary>
        /// Event that triggers when the websocket was successfully connected
        /// </summary>
        public event AsyncEventHandler<WebsocketConnectedArgs>? WebsocketConnected;
        /// <summary>
        /// Event that triggers when the websocket disconnected
        /// </summary>
        public event AsyncEventHandler? WebsocketDisconnected;
        /// <summary>
        /// Event that triggers when an error occurred on the websocket
        /// </summary>
        public event AsyncEventHandler<ErrorOccuredArgs>? ErrorOccurred;
        /// <summary>
        /// Event that triggers when the websocket was successfully reconnected
        /// </summary>
        public event AsyncEventHandler? WebsocketReconnected;

        /// <summary>
        /// Event that triggers on "channel.ad_break.begin" notifications
        /// </summary>
        public event AsyncEventHandler<ChannelAdBreakBeginArgs>? ChannelAdBreakBegin;

        /// <summary>
        /// Event that triggers on "channel.ban" notifications
        /// </summary>
        public event AsyncEventHandler<ChannelBanArgs>? ChannelBan;

        /// <summary>
        /// Event that triggers on "channel.charity_campaign.start" notifications
        /// </summary>
        public event AsyncEventHandler<ChannelCharityCampaignStartArgs>? ChannelCharityCampaignStart;
        /// <summary>
        /// Event that triggers on "channel.charity_campaign.donate" notifications
        /// </summary>
        public event AsyncEventHandler<ChannelCharityCampaignDonateArgs>? ChannelCharityCampaignDonate;
        /// <summary>
        /// Event that triggers on "channel.charity_campaign.progress" notifications
        /// </summary>
        public event AsyncEventHandler<ChannelCharityCampaignProgressArgs>? ChannelCharityCampaignProgress;
        /// <summary>
        /// Event that triggers on "channel.charity_campaign.stop" notifications
        /// </summary>
        public event AsyncEventHandler<ChannelCharityCampaignStopArgs>? ChannelCharityCampaignStop;

        /// <summary>
        /// Event that triggers on channel.chat.clear notifications
        /// </summary>
        public event AsyncEventHandler<ChannelChatClearArgs>? ChannelChatClear;
        /// <summary>
        /// Event that triggers on channel.chat.clear_user_messages notifications
        /// </summary>
        public event AsyncEventHandler<ChannelChatClearUserMessagesArgs>? ChannelChatClearUserMessages;
        /// <summary>
        /// Event that triggers on channel.chat.message notifications
        /// </summary>
        public event AsyncEventHandler<ChannelChatMessageArgs>? ChannelChatMessage;
        /// <summary>
        /// Event that triggers on "channel.chat.message_delete" notifications
        /// </summary>
        public event AsyncEventHandler<ChannelChatMessageDeleteArgs>? ChannelChatMessageDelete;
        /// <summary>
        /// Event that triggers on "channel.chat.notification" notifications
        /// </summary>
        public event AsyncEventHandler<ChannelChatNotificationArgs>? ChannelChatNotification;
        /// <summary>
        /// Event that triggers on "channel.cheer" notifications
        /// </summary>
        public event AsyncEventHandler<ChannelCheerArgs>? ChannelCheer;
        /// <summary>
        /// Event that triggers on "channel.follow" notifications
        /// </summary>
        public event AsyncEventHandler<ChannelFollowArgs>? ChannelFollow;

        /// <summary>
        /// Event that triggers on "channel.goal.begin" notifications
        /// </summary>
        public event AsyncEventHandler<ChannelGoalBeginArgs>? ChannelGoalBegin;
        /// <summary>
        /// Event that triggers on "channel.goal.end" notifications
        /// </summary>
        public event AsyncEventHandler<ChannelGoalEndArgs>? ChannelGoalEnd;
        /// <summary>
        /// Event that triggers on "channel.goal.progress" notifications
        /// </summary>
        public event AsyncEventHandler<ChannelGoalProgressArgs>? ChannelGoalProgress;

        /// <summary>
        /// Event that triggers on "channel.guest_star_guest.update" notifications
        /// </summary>
        public event AsyncEventHandler<ChannelGuestStarGuestUpdateArgs>? ChannelGuestStarGuestUpdate;
        /// <summary>
        /// Event that triggers on "channel.guest_star_session.begin" notifications
        /// </summary>
        public event AsyncEventHandler<ChannelGuestStarSessionBegin>? ChannelGuestStarSessionBegin;
        /// <summary>
        /// Event that triggers on "channel.guest_star_guest.update" notifications
        /// </summary>
        public event AsyncEventHandler<ChannelGuestStarSessionEnd>? ChannelGuestStarSessionEnd;
        /// <summary>
        /// Event that triggers on "channel.guest_star_settings.update" notifications
        /// </summary>
        public event AsyncEventHandler<ChannelGuestStarSettingsUpdateArgs>? ChannelGuestStarSettingsUpdate;
        /// <summary>
        /// Event that triggers on "channel.guest_star_slot.update" notifications
        /// </summary>
        public event AsyncEventHandler<ChannelGuestStarSlotUpdateArgs>? ChannelGuestStarSlotUpdate;

        /// <summary>
        /// Event that triggers on "channel.hype_train.begin" notifications
        /// </summary>
        public event AsyncEventHandler<ChannelHypeTrainBeginArgs>? ChannelHypeTrainBegin;
        /// <summary>
        /// Event that triggers on "channel.hype_train.end" notifications
        /// </summary>
        public event AsyncEventHandler<ChannelHypeTrainEndArgs>? ChannelHypeTrainEnd;
        /// <summary>
        /// Event that triggers on "channel.hype_train.progress" notifications
        /// </summary>
        public event AsyncEventHandler<ChannelHypeTrainProgressArgs>? ChannelHypeTrainProgress;

        /// <summary>
        /// Event that triggers on "channel.moderator.add" notifications
        /// </summary>
        public event AsyncEventHandler<ChannelModeratorArgs>? ChannelModeratorAdd;
        /// <summary>
        /// Event that triggers on "channel.moderator.remove" notifications
        /// </summary>
        public event AsyncEventHandler<ChannelModeratorArgs>? ChannelModeratorRemove;

        /// <summary>
        /// Event that triggers on "channel.vip.add" notifications
        /// </summary>
        public event AsyncEventHandler<ChannelVipArgs>? ChannelVipAdd;
        /// <summary>
        /// Event that triggers on "channel.vip.remove" notifications
        /// </summary>
        public event AsyncEventHandler<ChannelVipArgs>? ChannelVipRemove;

        /// <summary>
        /// Event that triggers on "channel.channel_points_custom_reward.add" notifications
        /// </summary>
        public event AsyncEventHandler<ChannelPointsCustomRewardArgs>? ChannelPointsCustomRewardAdd;
        /// <summary>
        /// Event that triggers on "channel.channel_points_custom_reward.remove" notifications
        /// </summary>
        public event AsyncEventHandler<ChannelPointsCustomRewardArgs>? ChannelPointsCustomRewardRemove;
        /// <summary>
        /// Event that triggers on "channel.channel_points_custom_reward.update" notifications
        /// </summary>
        public event AsyncEventHandler<ChannelPointsCustomRewardArgs>? ChannelPointsCustomRewardUpdate;

        /// <summary>
        /// Event that triggers on "channel.channel_points_automatic_reward_redemption.add" notifications
        /// </summary>
        public event AsyncEventHandler<ChannelPointsAutomaticRewardRedemptionArgs>? ChannelPointsAutomaticRewardRedemptionAdd;

        /// <summary>
        /// Event that triggers on "channel.channel_points_custom_reward_redemption.add" notifications
        /// </summary>
        public event AsyncEventHandler<ChannelPointsCustomRewardRedemptionArgs>? ChannelPointsCustomRewardRedemptionAdd;
        /// <summary>
        /// Event that triggers on "channel.channel_points_custom_reward_redemption.update" notifications
        /// </summary>
        public event AsyncEventHandler<ChannelPointsCustomRewardRedemptionArgs>? ChannelPointsCustomRewardRedemptionUpdate;

        /// <summary>
        /// Event that triggers on "channel.poll.begin" notifications
        /// </summary>
        public event AsyncEventHandler<ChannelPollBeginArgs>? ChannelPollBegin;
        /// <summary>
        /// Event that triggers on "channel.poll.end" notifications
        /// </summary>
        public event AsyncEventHandler<ChannelPollEndArgs>? ChannelPollEnd;
        /// <summary>
        /// Event that triggers on "channel.poll.progress" notifications
        /// </summary>
        public event AsyncEventHandler<ChannelPollProgressArgs>? ChannelPollProgress;

        /// <summary>
        /// Event that triggers on "channel.prediction.begin" notifications
        /// </summary>
        public event AsyncEventHandler<ChannelPredictionBeginArgs>? ChannelPredictionBegin;
        /// <summary>
        /// Event that triggers on "channel.prediction.end" notifications
        /// </summary>
        public event AsyncEventHandler<ChannelPredictionEndArgs>? ChannelPredictionEnd;
        /// <summary>
        /// Event that triggers on "channel.prediction.lock" notifications
        /// </summary>
        public event AsyncEventHandler<ChannelPredictionLockArgs>? ChannelPredictionLock;
        /// <summary>
        /// Event that triggers on "channel.prediction.progress" notifications
        /// </summary>
        public event AsyncEventHandler<ChannelPredictionProgressArgs>? ChannelPredictionProgress;

        /// <summary>
        /// Event that triggers on "channel.raid" notifications
        /// </summary>
        public event AsyncEventHandler<ChannelRaidArgs>? ChannelRaid;

        /// <summary>
        /// Event that triggers on "channel.shield_mode.begin" notifications
        /// </summary>
        public event AsyncEventHandler<ChannelShieldModeBeginArgs>? ChannelShieldModeBegin;
        /// <summary>
        /// Event that triggers on "channel.shield_mode.end" notifications
        /// </summary>
        public event AsyncEventHandler<ChannelShieldModeEndArgs>? ChannelShieldModeEnd;

        /// <summary>
        /// Event that triggers on "channel.shoutout.create" notifications
        /// </summary>
        public event AsyncEventHandler<ChannelShoutoutCreateArgs>? ChannelShoutoutCreate;
        /// <summary>
        /// Event that triggers on "channel.shoutout.receive" notifications
        /// </summary>
        public event AsyncEventHandler<ChannelShoutoutReceiveArgs>? ChannelShoutoutReceive;

        /// <summary>
        /// Event that triggers on "channel.subscribe" notifications
        /// </summary>
        public event AsyncEventHandler<ChannelSubscribeArgs>? ChannelSubscribe;
        /// <summary>
        /// Event that triggers on "channel.subscription.end" notifications
        /// </summary>
        public event AsyncEventHandler<ChannelSubscriptionEndArgs>? ChannelSubscriptionEnd;
        /// <summary>
        /// Event that triggers on "channel.subscription.gift" notifications
        /// </summary>
        public event AsyncEventHandler<ChannelSubscriptionGiftArgs>? ChannelSubscriptionGift;
        /// <summary>
        /// Event that triggers on "channel.subscription.message" notifications
        /// </summary>
        public event AsyncEventHandler<ChannelSubscriptionMessageArgs>? ChannelSubscriptionMessage;

        /// <summary>
        /// Event that triggers on "channel.suspicious_user.message" notifications
        /// </summary>
        public event AsyncEventHandler<ChannelSuspiciousUserMessageArgs>? ChannelSuspiciousUserMessage;

        /// <summary>
        /// Event that triggers on "channel.suspicious_user.update" notifications
        /// </summary>
        public event AsyncEventHandler<ChannelSuspiciousUserUpdateArgs>? ChannelSuspiciousUserUpdate;

        /// <summary>
        /// Event that triggers on "channel.warning.acknowledge" notifications
        /// </summary>
        public event AsyncEventHandler<ChannelWarningAcknowledgeArgs>? ChannelWarningAcknowledge;

        /// <summary>
        /// Event that triggers on "channel.warning.send" notifications
        /// </summary>
        public event AsyncEventHandler<ChannelWarningSendArgs>? ChannelWarningSend;

        /// <summary>
        /// Event that triggers on "channel.unban" notifications
        /// </summary>
        public event AsyncEventHandler<ChannelUnbanArgs>? ChannelUnban;

        /// <summary>
        /// Event that triggers on "channel.update" notifications
        /// </summary>
        public event AsyncEventHandler<ChannelUpdateArgs>? ChannelUpdate;

        /// <summary>
        /// Event that triggers on "stream.offline" notifications
        /// </summary>
        public event AsyncEventHandler<StreamOfflineArgs>? StreamOffline;
        /// <summary>
        /// Event that triggers on "stream.online" notifications
        /// </summary>
        public event AsyncEventHandler<StreamOnlineArgs>? StreamOnline;

        /// <summary>
        /// Event that triggers on "user.update" notifications
        /// </summary>
        public event AsyncEventHandler<UserUpdateArgs>? UserUpdate;

        /// <summary>
        /// Event that triggers on "user.whisper.message" notifications
        /// </summary>
        public event AsyncEventHandler<UserWhisperMessageArgs>? UserWhisperMessage;
        
        /// <summary>
        /// Event that triggers on "channel.shared_chat.begin" notifications
        /// </summary>
        public event AsyncEventHandler<ChannelSharedChatSessionBeginArgs>? ChannelSharedChatSessionBegin;
        
        /// <summary>
        /// Event that triggers on "channel.shared_chat.update" notifications
        /// </summary>
        public event AsyncEventHandler<ChannelSharedChatSessionUpdateArgs>? ChannelSharedChatSessionUpdate;
        
        /// <summary>
        /// Event that triggers on "channel.shared_chat.end" notifications
        /// </summary>
        public event AsyncEventHandler<ChannelSharedChatSessionEndArgs>? ChannelSharedChatSessionEnd;

        /// <summary>
        /// Event that triggers on "channel.unban_request.create" notifications
        /// </summary>
        public event AsyncEventHandler<ChannelUnbanRequestCreateArgs>? ChannelUnbanRequestCreate;

        /// <summary>
        /// Event that triggers on "channel.unban_request.resolve" notifications
        /// </summary>
        public event AsyncEventHandler<ChannelUnbanRequestResolveArgs>? ChannelUnbanRequestResolve;

        #endregion

        /// <summary>
        /// Id associated with the Websocket Session. Needed for creating subscriptions for the socket.
        /// </summary>
        public string SessionId { get; private set; } = string.Empty;

        private CancellationTokenSource? _cts;

        private DateTimeOffset _lastReceived = DateTimeOffset.MinValue;
        private TimeSpan _keepAliveTimeout = TimeSpan.Zero;

        private bool _reconnectRequested;
        private bool _reconnectComplete;

        private WebsocketClient _websocketClient;

        private readonly Dictionary<string, Action<EventSubWebsocketClient, string, JsonSerializerOptions>> _handlers = new();
        private readonly ILogger<EventSubWebsocketClient> _logger;
        private readonly ILoggerFactory? _loggerFactory;
        private readonly IServiceProvider? _serviceProvider;

        private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            DictionaryKeyPolicy = JsonNamingPolicy.SnakeCaseLower
        };

        private const string WEBSOCKET_URL = "wss://eventsub.wss.twitch.tv/ws";

        /// <summary>
        /// Instantiates an EventSubWebsocketClient used to subscribe to EventSub notifications via Websockets.
        /// </summary>
        /// <param name="logger">Logger for the EventSubWebsocketClient</param>
        /// <param name="handlers">Enumerable of SubscriptionType handlers</param>
        /// <param name="serviceProvider">DI Container to resolve other dependencies dynamically</param>
        /// <param name="websocketClient">Underlying Websocket client to connect to connect to EventSub Websocket service</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException if a dependency is null</exception>
        public EventSubWebsocketClient(ILogger<EventSubWebsocketClient> logger, IEnumerable<INotificationHandler> handlers, IServiceProvider serviceProvider, WebsocketClient websocketClient)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

            _websocketClient = websocketClient ?? throw new ArgumentNullException(nameof(websocketClient));
            _websocketClient.OnDataReceived += OnDataReceived;
            _websocketClient.OnErrorOccurred += OnErrorOccurred;

            PrepareHandlers(handlers);

            _reconnectComplete = false;
            _reconnectRequested = false;
        }

        /// <summary>
        /// Instantiates an EventSubWebsocketClient used to subscribe to EventSub notifications via Websockets.
        /// </summary>
        /// <param name="loggerFactory">LoggerFactory used to construct Loggers for the EventSubWebsocketClient and underlying classes</param>
        public EventSubWebsocketClient(ILoggerFactory? loggerFactory = null)
        {
            _loggerFactory = loggerFactory;

            _logger = _loggerFactory != null
                ? _loggerFactory.CreateLogger<EventSubWebsocketClient>()
                : NullLogger<EventSubWebsocketClient>.Instance;

            _websocketClient = _loggerFactory != null
                ? new WebsocketClient(_loggerFactory.CreateLogger<WebsocketClient>())
                : new WebsocketClient();

            _websocketClient.OnDataReceived += OnDataReceived;
            _websocketClient.OnErrorOccurred += OnErrorOccurred;

            var handlers = typeof(INotificationHandler)
                .Assembly.ExportedTypes
                .Where(x => typeof(INotificationHandler).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                .Select(Activator.CreateInstance).Cast<INotificationHandler>()
                .ToList();

            PrepareHandlers(handlers);

            _reconnectComplete = false;
            _reconnectRequested = false;
        }

        /// <summary>
        /// Connect to Twitch EventSub Websockets
        /// </summary>
        /// <param name="url">Optional url param to be able to connect to reconnect urls provided by Twitch or test servers</param>
        /// <returns>true: Connection successful false: Connection failed</returns>
        public async Task<bool> ConnectAsync(Uri? url = null)
        {
            url = url ?? new Uri(WEBSOCKET_URL);
            _lastReceived = DateTimeOffset.MinValue;

            var success = await _websocketClient.ConnectAsync(url);

            if (!success)
                return false;

            _cts = new CancellationTokenSource();

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Task.Factory.StartNew(ConnectionCheckAsync, _cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            return true;
        }

        /// <summary>
        /// Disconnect from Twitch EventSub Websockets
        /// </summary>
        /// <returns>true: Disconnect successful false: Disconnect failed</returns>
        public async Task<bool> DisconnectAsync()
        {
            _cts?.Cancel();
            return await _websocketClient.DisconnectAsync();
        }

        /// <summary>
        /// Reconnect to Twitch EventSub Websockets with a Twitch given Url
        /// </summary>
        /// <returns>true: Reconnect successful false: Reconnect failed</returns>
        public Task<bool> ReconnectAsync()
        {
            return ReconnectAsync(new Uri(WEBSOCKET_URL));
        }

        /// <summary>
        /// Reconnect to Twitch EventSub Websockets with a Twitch given Url
        /// </summary>
        /// <param name="url">New Websocket Url to connect to, to preserve current session and topics</param>
        /// <returns>true: Reconnect successful false: Reconnect failed</returns>
        private async Task<bool> ReconnectAsync(Uri url)
        {
            url = url ?? new Uri(WEBSOCKET_URL);

            if (_reconnectRequested)
            {

                var reconnectClient = _serviceProvider != null
                    ? _serviceProvider.GetRequiredService<WebsocketClient>()
                    : new WebsocketClient(_loggerFactory?.CreateLogger<WebsocketClient>());

                reconnectClient.OnDataReceived += OnDataReceived;
                reconnectClient.OnErrorOccurred += OnErrorOccurred;

                if (!await reconnectClient.ConnectAsync(url))
                    return false;


                for (var i = 0; i < 200; i++)
                {
                    if (_cts == null || _cts.IsCancellationRequested)
                        break;

                    if (_reconnectComplete)
                    {
                        var oldRunningClient = _websocketClient;
                        _websocketClient = reconnectClient;

                        if (oldRunningClient.IsConnected)
                            await oldRunningClient.DisconnectAsync();
                        oldRunningClient.Dispose();

                        await WebsocketReconnected.InvokeAsync(this, EventArgs.Empty);

                        _reconnectRequested = false;
                        _reconnectComplete = false;

                        return true;
                    }

                    await Task.Delay(100);
                }

                _logger?.LogReconnectFailed(SessionId);

                return false;
            }

            if (_websocketClient.IsConnected)
                await DisconnectAsync();

            _websocketClient.Dispose();

            _websocketClient = _serviceProvider != null
                ? _serviceProvider.GetRequiredService<WebsocketClient>()
                : new WebsocketClient(_loggerFactory?.CreateLogger<WebsocketClient>());

            _websocketClient.OnDataReceived += OnDataReceived;
            _websocketClient.OnErrorOccurred += OnErrorOccurred;

            if (!await ConnectAsync())
                return false;

            await WebsocketReconnected.InvokeAsync(this, EventArgs.Empty);

            return true;
        }

        /// <summary>
        /// Setup handlers for all supported subscription types
        /// </summary>
        /// <param name="handlers">Enumerable of handlers that are responsible for acting on a specified subscription type</param>
        private void PrepareHandlers(IEnumerable<INotificationHandler> handlers)
        {
            foreach (var handler in handlers)
            {
#if NET6_0_OR_GREATER
                _handlers.TryAdd(handler.SubscriptionType, handler.Handle);
#else
                if (!_handlers.ContainsKey(handler.SubscriptionType))
                    _handlers.Add(handler.SubscriptionType, handler.Handle);
#endif
            }
        }

        /// <summary>
        /// Background operation checking the client health based on the last received message and the Twitch specified minimum frequency + a 20% grace period.
        /// <para>E.g. a Twitch specified 10 seconds minimum frequency would result in 12 seconds used by the client to honor network latencies and so on.</para>
        /// </summary>
        /// <returns>a Task that represents the background operation</returns>
        private async Task ConnectionCheckAsync()
        {
            while (_cts != null && _websocketClient.IsConnected && !_cts.IsCancellationRequested)
            {
                if (_lastReceived != DateTimeOffset.MinValue)
                    if (_keepAliveTimeout != TimeSpan.Zero)
                        if (_lastReceived.Add(_keepAliveTimeout) < DateTimeOffset.Now)
                            break;

                await Task.Delay(TimeSpan.FromSeconds(1), _cts.Token);
            }

            await DisconnectAsync();

            await WebsocketDisconnected.InvokeAsync(this, EventArgs.Empty);
        }

        /// <summary>
        /// AsyncEventHandler for the DataReceived event from the underlying websocket. This is where every notification that gets in gets handled"/>
        /// </summary>
        /// <param name="sender">Sender of the event. In this case <see cref="WebsocketClient"/></param>
        /// <param name="e">EventArgs send with the event. <see cref="DataReceivedArgs"/></param>
        private async Task OnDataReceived(object sender, DataReceivedArgs e)
        {
            _logger?.LogMessage(e.Bytes);
            _lastReceived = DateTimeOffset.Now;

            var json = JsonDocument.Parse(e.Bytes);
            var metadata = json.RootElement.GetProperty("metadata"u8);
            var messageType = metadata.GetProperty("message_type"u8).GetString();
            switch (messageType)
            {
                case "session_welcome":
                    await HandleWelcome(e.Bytes);
                    break;
                case "session_disconnect":
                    await HandleDisconnect(e.Bytes);
                    break;
                case "session_reconnect":
                    HandleReconnect(e.Bytes);
                    break;
                case "session_keepalive":
                    HandleKeepAlive(e.Bytes);
                    break;
                case "notification":
                    var subscriptionType = metadata.GetProperty("subscription_type"u8).GetString();
                    if (string.IsNullOrWhiteSpace(subscriptionType))
                    {
                        await ErrorOccurred.InvokeAsync(this, new ErrorOccuredArgs { Exception = new ArgumentNullException(nameof(subscriptionType)), Message = "Unable to determine subscription type!" });
                        break;
                    }
                    var message1 = Encoding.UTF8.GetString(e.Bytes);
                    HandleNotification(message1, subscriptionType!);
                    break;
                case "revocation":
                    var message2 = Encoding.UTF8.GetString(e.Bytes);
                    HandleRevocation(message2);
                    break;
                default:
                    _logger?.LogUnknownMessageType(messageType);
                    break;
            }
        }

        /// <summary>
        /// AsyncEventHandler for the ErrorOccurred event from the underlying websocket. This handler only serves as a relay up to the user code"/>
        /// </summary>
        /// <param name="sender">Sender of the event. In this case <see cref="WebsocketClient"/></param>
        /// <param name="e">EventArgs send with the event. <see cref="ErrorOccuredArgs"/></param>
        private async Task OnErrorOccurred(object sender, ErrorOccuredArgs e)
        {
            await ErrorOccurred.InvokeAsync(this, e);
        }

        /// <summary>
        /// Handles 'session_reconnect' notifications
        /// </summary>
        /// <param name="message">notification message received from Twitch EventSub</param>
        private void HandleReconnect(byte[] message)
        {
            _logger?.LogReconnectRequested(SessionId);
            var data = JsonSerializer.Deserialize<EventSubWebsocketSessionInfoMessage>(message, _jsonSerializerOptions);
            _reconnectRequested = true;

            Task.Run(async () => await ReconnectAsync(new Uri(data?.Payload.Session.ReconnectUrl ?? WEBSOCKET_URL)));
        }

        /// <summary>
        /// Handles 'session_welcome' notifications
        /// </summary>
        /// <param name="message">notification message received from Twitch EventSub</param>
        private async ValueTask HandleWelcome(byte[] message)
        {
            var data = JsonSerializer.Deserialize<EventSubWebsocketSessionInfoMessage>(message, _jsonSerializerOptions);

            if (data is null)
                return;

            if (_reconnectRequested)
                _reconnectComplete = true;

            SessionId = data.Payload.Session.Id;
            var keepAliveTimeout = data.Payload.Session.KeepaliveTimeoutSeconds + data.Payload.Session.KeepaliveTimeoutSeconds * 0.2;

            _keepAliveTimeout = TimeSpan.FromSeconds(keepAliveTimeout ?? 10);

            await WebsocketConnected.InvokeAsync(this, new WebsocketConnectedArgs { IsRequestedReconnect = _reconnectRequested });
        }

        /// <summary>
        /// Handles 'session_disconnect' notifications
        /// </summary>
        /// <param name="message">notification message received from Twitch EventSub</param>
        private async Task HandleDisconnect(byte[] message)
        {
            var data = JsonSerializer.Deserialize<EventSubWebsocketSessionInfoMessage>(message);

            if (data != null)
                _logger?.LogForceDisconnected(data.Payload.Session.Id, data.Payload.Session.DisconnectedAt, data.Payload.Session.DisconnectReason);

            await WebsocketDisconnected.InvokeAsync(this, EventArgs.Empty);
        }

        /// <summary>
        /// Handles 'session_keepalive' notifications
        /// </summary>
        /// <param name="message">notification message received from Twitch EventSub</param>
        private void HandleKeepAlive(byte[] message)
        {
            _ = message;
        }

        /// <summary>
        /// Handles 'notification' notifications
        /// </summary>
        /// <param name="message">notification message received from Twitch EventSub</param>
        /// <param name="subscriptionType">subscription type received from Twitch EventSub</param>
        private void HandleNotification(string message, string subscriptionType)
        {
            if (_handlers.TryGetValue(subscriptionType, out var handler))
                handler(this, message, _jsonSerializerOptions);
        }

        /// <summary>
        /// Handles 'revocation' notifications
        /// </summary>
        /// <param name="message">notification message received from Twitch EventSub</param>
        private void HandleRevocation(string message)
        {
            if (_handlers.TryGetValue("revocation", out var handler))
                handler(this, message, _jsonSerializerOptions);
        }

        /// <summary>
        /// Raises an event from this class from a handler by reflection
        /// </summary>
        /// <param name="eventName">name of the event to raise</param>
        /// <param name="args">args to pass with the event</param>
        internal async void RaiseEvent(string eventName, object? args = null)
        {
            var fInfo = GetType().GetField(eventName, BindingFlags.Instance | BindingFlags.NonPublic);

            if (fInfo?.GetValue(this) is not MulticastDelegate multi)
                return;

            var parameters = new object[] { this, args ?? EventArgs.Empty };
            foreach (var del in multi.GetInvocationList())
            {
                try
                {
                    var result = del.Method.Invoke(del.Target, parameters);
                    if (result is Task task)
                        await task;
                }
                catch (Exception ex)
                {
                    _logger.LogRaiseEventExeption(eventName, ex);
                }
            }
        }
    }
}
