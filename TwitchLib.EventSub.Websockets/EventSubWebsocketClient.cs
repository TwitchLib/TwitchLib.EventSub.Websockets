using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TwitchLib.EventSub.Websockets.Client;
using TwitchLib.EventSub.Websockets.Core.EventArgs;
using TwitchLib.EventSub.Websockets.Core.EventArgs.Channel;
using TwitchLib.EventSub.Websockets.Core.EventArgs.Stream;
using TwitchLib.EventSub.Websockets.Core.EventArgs.User;
using TwitchLib.EventSub.Websockets.Core.Handler;
using TwitchLib.EventSub.Websockets.Core.Models;
using TwitchLib.EventSub.Websockets.Core.NamingPolicies;

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
        public event EventHandler<WebsocketConnectedArgs> WebsocketConnected;
        /// <summary>
        /// Event that triggers when the websocket disconnected
        /// </summary>
        public event EventHandler WebsocketDisconnected;
        /// <summary>
        /// Event that triggers when an error occurred on the websocket
        /// </summary>
        public event EventHandler<ErrorOccuredArgs> ErrorOccurred;
        /// <summary>
        /// Event that triggers when the websocket was successfully reconnected
        /// </summary>
        public event EventHandler WebsocketReconnected;

        /// <summary>
        /// Event that triggers on "channel.ban" notifications
        /// </summary>
        public event EventHandler<ChannelBanArgs> ChannelBan;
        /// <summary>
        /// Event that triggers on "channel.charity_campaign.donate" notifications
        /// </summary>
        public event EventHandler<ChannelCharityCampaignDonateArgs> ChannelCharityCampaignDonate;
        /// <summary>
        /// Event that triggers on "channel.cheer" notifications
        /// </summary>
        public event EventHandler<ChannelCheerArgs> ChannelCheer;
        /// <summary>
        /// Event that triggers on "channel.follow" notifications
        /// </summary>
        public event EventHandler<ChannelFollowArgs> ChannelFollow;

        /// <summary>
        /// Event that triggers on "channel.goal.begin" notifications
        /// </summary>
        public event EventHandler<ChannelGoalBeginArgs> ChannelGoalBegin;
        /// <summary>
        /// Event that triggers on "channel.goal.end" notifications
        /// </summary>
        public event EventHandler<ChannelGoalEndArgs> ChannelGoalEnd;
        /// <summary>
        /// Event that triggers on "channel.goal.progress" notifications
        /// </summary>
        public event EventHandler<ChannelGoalProgressArgs> ChannelGoalProgress;

        /// <summary>
        /// Event that triggers on "channel.hype_train.begin" notifications
        /// </summary>
        public event EventHandler<ChannelHypeTrainBeginArgs> ChannelHypeTrainBegin;
        /// <summary>
        /// Event that triggers on "channel.hype_train.end" notifications
        /// </summary>
        public event EventHandler<ChannelHypeTrainEndArgs> ChannelHypeTrainEnd;
        /// <summary>
        /// Event that triggers on "channel.hype_train.progress" notifications
        /// </summary>
        public event EventHandler<ChannelHypeTrainProgressArgs> ChannelHypeTrainProgress;

        /// <summary>
        /// Event that triggers on "channel.moderator.add" notifications
        /// </summary>
        public event EventHandler<ChannelModeratorArgs> ChannelModeratorAdd;
        /// <summary>
        /// Event that triggers on "channel.moderator.remove" notifications
        /// </summary
        public event EventHandler<ChannelModeratorArgs> ChannelModeratorRemove;

        /// <summary>
        /// Event that triggers on "channel.channel_points_custom_reward.add" notifications
        /// </summary>
        public event EventHandler<ChannelPointsCustomRewardArgs> ChannelPointsCustomRewardAdd;
        /// <summary>
        /// Event that triggers on "channel.channel_points_custom_reward.remove" notifications
        /// </summary>
        public event EventHandler<ChannelPointsCustomRewardArgs> ChannelPointsCustomRewardRemove;
        /// <summary>
        /// Event that triggers on "channel.channel_points_custom_reward.update" notifications
        /// </summary>
        public event EventHandler<ChannelPointsCustomRewardArgs> ChannelPointsCustomRewardUpdate;

        /// <summary>
        /// Event that triggers on "channel.channel_points_custom_reward_redemption.add" notifications
        /// </summary>
        public event EventHandler<ChannelPointsCustomRewardRedemptionArgs> ChannelPointsCustomRewardRedemptionAdd;
        /// <summary>
        /// Event that triggers on "channel.channel_points_custom_reward_redemption.update" notifications
        /// </summary>
        public event EventHandler<ChannelPointsCustomRewardRedemptionArgs> ChannelPointsCustomRewardRedemptionUpdate;

        /// <summary>
        /// Event that triggers on "channel.poll.begin" notifications
        /// </summary>
        public event EventHandler<ChannelPollBeginArgs> ChannelPollBegin;
        /// <summary>
        /// Event that triggers on "channel.poll.end" notifications
        /// </summary>
        public event EventHandler<ChannelPollEndArgs> ChannelPollEnd;
        /// <summary>
        /// Event that triggers on "channel.poll.progress" notifications
        /// </summary>
        public event EventHandler<ChannelPollProgressArgs> ChannelPollProgress;

        /// <summary>
        /// Event that triggers on "channel.prediction.begin" notifications
        /// </summary>
        public event EventHandler<ChannelPredictionBeginArgs> ChannelPredictionBegin;
        /// <summary>
        /// Event that triggers on "channel.prediction.end" notifications
        /// </summary>
        public event EventHandler<ChannelPredictionEndArgs> ChannelPredictionEnd;
        /// <summary>
        /// Event that triggers on "channel.prediction.lock" notifications
        /// </summary>
        public event EventHandler<ChannelPredictionLockArgs> ChannelPredictionLock;
        /// <summary>
        /// Event that triggers on "channel.prediction.progress" notifications
        /// </summary>
        public event EventHandler<ChannelPredictionProgressArgs> ChannelPredictionProgress;

        /// <summary>
        /// Event that triggers on "channel.raid" notifications
        /// </summary>
        public event EventHandler<ChannelRaidArgs> ChannelRaid;

        /// <summary>
        /// Event that triggers on "channel.subscribe" notifications
        /// </summary>
        public event EventHandler<ChannelSubscribeArgs> ChannelSubscribe;
        /// <summary>
        /// Event that triggers on "channel.subscription.end" notifications
        /// </summary>
        public event EventHandler<ChannelSubscriptionEndArgs> ChannelSubscriptionEnd;
        /// <summary>
        /// Event that triggers on "channel.subscription.gift" notifications
        /// </summary>
        public event EventHandler<ChannelSubscriptionGiftArgs> ChannelSubscriptionGift;
        /// <summary>
        /// Event that triggers on "channel.subscription.message" notifications
        /// </summary>
        public event EventHandler<ChannelSubscriptionMessageArgs> ChannelSubscriptionMessage;

        /// <summary>
        /// Event that triggers on "channel.unban" notifications
        /// </summary>
        public event EventHandler<ChannelUnbanArgs> ChannelUnban;

        /// <summary>
        /// Event that triggers on "channel.update" notifications
        /// </summary>
        public event EventHandler<ChannelUpdateArgs> ChannelUpdate;

        /// <summary>
        /// Event that triggers on "stream.offline" notifications
        /// </summary>
        public event EventHandler<StreamOfflineArgs> StreamOffline;
        /// <summary>
        /// Event that triggers on "stream.online" notifications
        /// </summary>
        public event EventHandler<StreamOnlineArgs> StreamOnline;

        /// <summary>
        /// Event that triggers on "user.update" notifications
        /// </summary>
        public event EventHandler<UserUpdateArgs> UserUpdate;

        #endregion

        /// <summary>
        /// Id associated with the Websocket Session. Needed for creating subscriptions for the socket.
        /// </summary>
        public string SessionId { get; private set; }

        private CancellationTokenSource _cts;

        private DateTimeOffset _lastReceived = DateTimeOffset.MinValue;
        private TimeSpan _keepAliveTimeout = TimeSpan.Zero;

        private bool _reconnectRequested;
        private bool _reconnectComplete;

        private WebsocketClient _websocketClient;
        private Dictionary<string, Action<EventSubWebsocketClient, string, JsonSerializerOptions>> _handlers;

        private readonly ILogger<EventSubWebsocketClient> _logger;
        private readonly IServiceProvider _serviceProvider;

        private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = new SnakeCaseNamingPolicy(),
            DictionaryKeyPolicy = new SnakeCaseNamingPolicy()
        };

        private const string WEBSOCKET_URL = "wss://eventsub-beta.wss.twitch.tv/ws";

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
        /// Connect to Twitch EventSub Websockets
        /// </summary>
        /// <param name="url">Optional url param to be able to connect to reconnect urls provided by Twitch or test servers</param>
        /// <returns>true: Connection successful false: Connection failed</returns>
        public async Task<bool> ConnectAsync(Uri url = null)
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
                    : new WebsocketClient(null);

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

                        WebsocketReconnected?.Invoke(this, EventArgs.Empty);

                        _reconnectRequested = false;
                        _reconnectComplete = false;

                        return true;
                    }

                    await Task.Delay(100);
                }

                _logger?.LogError($"Websocket reconnect for {SessionId} failed!");

                return false;
            }

            if (_websocketClient.IsConnected)
                await DisconnectAsync();

            _websocketClient.Dispose();

            _websocketClient = _serviceProvider != null
                ? _serviceProvider.GetRequiredService<WebsocketClient>()
                : new WebsocketClient(null);

            _websocketClient.OnDataReceived += OnDataReceived;
            _websocketClient.OnErrorOccurred += OnErrorOccurred;

            if (!await ConnectAsync())
                return false;

            WebsocketReconnected?.Invoke(this, EventArgs.Empty);

            return true;
        }

        /// <summary>
        /// Setup handlers for all supported subscription types
        /// </summary>
        /// <param name="handlers">Enumerable of handlers that are responsible for acting on a specified subscription type</param>
        private void PrepareHandlers(IEnumerable<INotificationHandler> handlers)
        {
            _handlers = _handlers ?? new Dictionary<string, Action<EventSubWebsocketClient, string, JsonSerializerOptions>>();

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

            WebsocketDisconnected?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// EventHandler for the DataReceived event from the underlying websocket. This is where every notification that gets in gets handled"/>
        /// </summary>
        /// <param name="sender">Sender of the event. In this case <see cref="WebsocketClient"/></param>
        /// <param name="e">EventArgs send with the event. <see cref="DataReceivedArgs"/></param>
        private void OnDataReceived(object sender, DataReceivedArgs e)
        {
            _lastReceived = DateTimeOffset.Now;

            var json = JsonDocument.Parse(e.Message);
            var messageType = json.RootElement.GetProperty("metadata").GetProperty("message_type").GetString();
            switch (messageType)
            {
                case "session_welcome":
                    HandleWelcome(e.Message);
                    break;
                case "session_disconnect":
                    HandleDisconnect(e.Message);
                    break;
                case "session_reconnect":
                    HandleReconnect(e.Message);
                    break;
                case "session_keepalive":
                    HandleKeepAlive(e.Message);
                    break;
                case "notification":
                    var subscriptionType = json.RootElement.GetProperty("metadata").GetProperty("subscription_type").GetString();
                    if (string.IsNullOrWhiteSpace(subscriptionType))
                    {
                        ErrorOccurred?.Invoke(this, new ErrorOccuredArgs { Exception = new ArgumentNullException(nameof(subscriptionType)), Message = "Unable to determine subscription type!" });
                        break;
                    }
                    HandleNotification(e.Message, subscriptionType);
                    break;
                case "revocation":
                    HandleRevocation(e.Message);
                    break;
                default:
                    _logger?.LogWarning($"Unknown message type: {messageType}");
                    _logger?.LogDebug(e.Message);
                    break;
            }
        }

        /// <summary>
        /// EventHandler for the ErrorOccurred event from the underlying websocket. This handler only serves as a relay up to the user code"/>
        /// </summary>
        /// <param name="sender">Sender of the event. In this case <see cref="WebsocketClient"/></param>
        /// <param name="e">EventArgs send with the event. <see cref="ErrorOccuredArgs"/></param>
        private void OnErrorOccurred(object sender, ErrorOccuredArgs e)
        {
            ErrorOccurred?.Invoke(this, e);
        }

        /// <summary>
        /// Handles 'session_reconnect' notifications
        /// </summary>
        /// <param name="message">notification message received from Twitch EventSub</param>
        private void HandleReconnect(string message)
        {
            _logger?.LogWarning($"Reconnect for {SessionId} requested!");
            var data = JsonSerializer.Deserialize<EventSubWebsocketSessionInfoMessage>(message, _jsonSerializerOptions);
            _reconnectRequested = true;

            Task.Run(async () => await ReconnectAsync(new Uri(data?.Payload.Session.ReconnectUrl ?? WEBSOCKET_URL)));

            _logger?.LogDebug(message);
        }

        /// <summary>
        /// Handles 'session_welcome' notifications
        /// </summary>
        /// <param name="message">notification message received from Twitch EventSub</param>
        private void HandleWelcome(string message)
        {
            var data = JsonSerializer.Deserialize<EventSubWebsocketSessionInfoMessage>(message, _jsonSerializerOptions);

            if (data is null)
                return;

            if (_reconnectRequested)
                _reconnectComplete = true;

            SessionId = data.Payload.Session.Id;
            var keepAliveTimeout = data.Payload.Session.KeepaliveTimeoutSeconds + data.Payload.Session.KeepaliveTimeoutSeconds * 0.2;

            _keepAliveTimeout = keepAliveTimeout.HasValue ? TimeSpan.FromSeconds(keepAliveTimeout.Value) : TimeSpan.FromSeconds(10);

            WebsocketConnected?.Invoke(this, new WebsocketConnectedArgs { IsRequestedReconnect = _reconnectRequested });

            _logger?.LogDebug(message);
        }

        /// <summary>
        /// Handles 'session_disconnect' notifications
        /// </summary>
        /// <param name="message">notification message received from Twitch EventSub</param>
        private void HandleDisconnect(string message)
        {
            var data = JsonSerializer.Deserialize<EventSubWebsocketSessionInfoMessage>(message);

            if (data != null)
                _logger?.LogCritical($"Websocket {data.Payload.Session.Id} disconnected at {data.Payload.Session.DisconnectedAt}. Reason: {data.Payload.Session.DisconnectReason}");

            WebsocketDisconnected?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Handles 'session_keepalive' notifications
        /// </summary>
        /// <param name="message">notification message received from Twitch EventSub</param>
        private void HandleKeepAlive(string message)
        {
            _logger?.LogDebug(message);
        }

        /// <summary>
        /// Handles 'notification' notifications
        /// </summary>
        /// <param name="message">notification message received from Twitch EventSub</param>
        /// <param name="subscriptionType">subscription type received from Twitch EventSub</param>
        private void HandleNotification(string message, string subscriptionType)
        {
            if (_handlers != null && _handlers.TryGetValue(subscriptionType, out var handler))
                handler(this, message, _jsonSerializerOptions);

            _logger?.LogDebug(message);
        }

        /// <summary>
        /// Handles 'revocation' notifications
        /// </summary>
        /// <param name="message">notification message received from Twitch EventSub</param>
        private void HandleRevocation(string message)
        {
            if (_handlers != null && _handlers.TryGetValue("revocation", out var handler))
                handler(this, message, _jsonSerializerOptions);

            _logger?.LogDebug(message);
        }

        /// <summary>
        /// Raises an event from this class from a handler by reflection
        /// </summary>
        /// <param name="eventName">name of the event to raise</param>
        /// <param name="args">args to pass with the event</param>
        internal void RaiseEvent(string eventName, object args = null)
        {
            var fInfo = GetType().GetField(eventName, BindingFlags.Instance | BindingFlags.NonPublic);

            var multi = fInfo?.GetValue(this) as MulticastDelegate;
            if (multi is null)
                return;

            foreach (var del in multi.GetInvocationList())
            {
                del.Method.Invoke(del.Target, args == null ? new object[] { this, EventArgs.Empty } : new[] { this, args });
            }
        }
    }
}