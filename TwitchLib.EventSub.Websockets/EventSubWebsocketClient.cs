using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TwitchLib.EventSub.Core;
using TwitchLib.EventSub.Core.EventArgs;
using TwitchLib.EventSub.Core.EventArgs.Automod;
using TwitchLib.EventSub.Core.EventArgs.Channel;
using TwitchLib.EventSub.Core.EventArgs.Conduit;
using TwitchLib.EventSub.Core.EventArgs.Stream;
using TwitchLib.EventSub.Core.EventArgs.User;
using TwitchLib.EventSub.Core.Extensions;
using TwitchLib.EventSub.Core.Models;
using TwitchLib.EventSub.Core.SubscriptionTypes.Automod;
using TwitchLib.EventSub.Core.SubscriptionTypes.Channel;
using TwitchLib.EventSub.Core.SubscriptionTypes.Conduit;
using TwitchLib.EventSub.Core.SubscriptionTypes.Stream;
using TwitchLib.EventSub.Core.SubscriptionTypes.User;
using TwitchLib.EventSub.Websockets.Client;
using TwitchLib.EventSub.Websockets.Core.EventArgs;
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
        public event AsyncEventHandler<WebsocketDisconnectedArgs>? WebsocketDisconnected;
        /// <summary>
        /// Event that triggers when an error occurred on the websocket
        /// </summary>
        public event AsyncEventHandler<ErrorOccuredArgs>? ErrorOccurred;
        /// <summary>
        /// Event that triggers when the websocket was successfully reconnected
        /// </summary>
        public event AsyncEventHandler<WebsocketReconnectedArgs>? WebsocketReconnected;
        /// <summary>
        /// Event that triggers when the websocket received revocation event
        /// </summary>
        public event AsyncEventHandler<RevocationArgs>? Revocation;

        /// <summary>
        /// Event that triggers when EventSub send notification, that's unknown. (ie.: not implementet ... yet!)
        /// </summary>
        public event AsyncEventHandler<UnknownEventSubNotificationArgs>? UnknownEventSubNotification;
        /// <summary>
        /// Event that triggers on "automod.message.hold" notifications
        /// </summary>
        public event AsyncEventHandler<AutomodMessageHoldArgs>? AutomodMessageHold;
        /// <summary>
        /// Event that triggers on "automod.message.hold" notifications
        /// </summary>
        public event AsyncEventHandler<AutomodMessageHoldV2Args>? AutomodMessageHoldV2;
        /// <summary>
        /// Event that triggers on "automod.message.update" notifications
        /// </summary>
        public event AsyncEventHandler<AutomodMessageUpdateArgs>? AutomodMessageUpdate;
        /// <summary>
        /// Event that triggers on "automod.message.update" notifications
        /// </summary>
        public event AsyncEventHandler<AutomodMessageUpdateV2Args>? AutomodMessageUpdateV2;
        /// <summary>
        /// Event that triggers on "automod.settings.update" notifications
        /// </summary>
        public event AsyncEventHandler<AutomodSettingsUpdateArgs>? AutomodSettingsUpdate;
        /// <summary>
        /// Event that triggers on "automod.terms.update" notifications
        /// </summary>
        public event AsyncEventHandler<AutomodTermsUpdateArgs>? AutomodTermsUpdate;
        /// <summary>
        /// Event that triggers on "channel.bits.use" notifications
        /// </summary>
        public event AsyncEventHandler<ChannelBitsUseArgs>? ChannelBitsUse;

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
        /// Event that triggers on "channel.chat_settings.update" notifications
        /// </summary>
        public event AsyncEventHandler<ChannelChatSettingsUpdateArgs>? ChannelChatSettingsUpdate;
        /// <summary>
        /// Event that triggers on "channel.chat.user_message_hold" notifications
        /// </summary>
        public event AsyncEventHandler<ChannelChatUserMessageHoldArgs>? ChannelChatUserMessageHold;
        /// <summary>
        /// Event that triggers on "channel.chat.user_message_update" notifications
        /// </summary>
        public event AsyncEventHandler<ChannelChatUserMessageUpdateArgs>? ChannelChatUserMessageUpdate;
        /// <summary>
        /// Event that triggers on "channel.cheer" notifications
        /// </summary>
        public event AsyncEventHandler<ChannelCheerArgs>? ChannelCheer;
        /// <summary>
        /// Event that triggers on "channel.follow" notifications
        /// </summary>
        public event AsyncEventHandler<ChannelFollowArgs>? ChannelFollow;

        /// <summary>
        /// Event that triggers on "conduit.shard.disabled" notifications
        /// </summary>
        public event AsyncEventHandler<ConduitShardDisabledArgs>? ConduitShardDisabled;

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
        public event AsyncEventHandler<ChannelGuestStarSessionBeginArgs>? ChannelGuestStarSessionBegin;
        /// <summary>
        /// Event that triggers on "channel.guest_star_guest.update" notifications
        /// </summary>
        public event AsyncEventHandler<ChannelGuestStarSessionEndArgs>? ChannelGuestStarSessionEnd;
        /// <summary>
        /// Event that triggers on "channel.guest_star_settings.update" notifications
        /// </summary>
        public event AsyncEventHandler<ChannelGuestStarSettingsUpdateArgs>? ChannelGuestStarSettingsUpdate;
        /// <summary>
        /// Event that triggers on "channel.hype_train.begin" notifications
        /// </summary>
        public event AsyncEventHandler<ChannelHypeTrainBeginV2Args>? ChannelHypeTrainBeginV2;
        /// <summary>
        /// Event that triggers on "channel.hype_train.end" notifications
        /// </summary>
        public event AsyncEventHandler<ChannelHypeTrainEndV2Args>? ChannelHypeTrainEndV2;
        /// <summary>
        /// Event that triggers on "channel.hype_train.progress" notifications
        /// </summary>
        public event AsyncEventHandler<ChannelHypeTrainProgressV2Args>? ChannelHypeTrainProgressV2;

        /// <summary>
        /// Event that triggers on "channel.moderate" notifications
        /// </summary>
        public event AsyncEventHandler<ChannelModerateArgs>? ChannelModerate;
        /// <summary>
        /// Event that triggers on "channel.moderate" notifications
        /// </summary>
        public event AsyncEventHandler<ChannelModerateV2Args>? ChannelModerateV2;
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
        /// Event that triggers on "channel.channel_points_automatic_reward_redemption.add" notifications
        /// </summary>
        public event AsyncEventHandler<ChannelPointsAutomaticRewardRedemptionAddV2Args>? ChannelPointsAutomaticRewardRedemptionAddV2;

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

        private readonly ILogger<EventSubWebsocketClient> _logger;
        private readonly ILoggerFactory _loggerFactory;
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
        /// <param name="serviceProvider">DI Container to resolve other dependencies dynamically</param>
        /// <param name="websocketClient">Underlying Websocket client to connect to connect to EventSub Websocket service</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException if a dependency is null</exception>
        public EventSubWebsocketClient(ILogger<EventSubWebsocketClient> logger, IServiceProvider serviceProvider, WebsocketClient websocketClient)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

            _websocketClient = websocketClient ?? throw new ArgumentNullException(nameof(websocketClient));
            _websocketClient.OnDataReceived += OnDataReceived;
            _websocketClient.OnErrorOccurred += OnErrorOccurred;

            _reconnectComplete = false;
            _reconnectRequested = false;
        }

        /// <summary>
        /// Instantiates an EventSubWebsocketClient used to subscribe to EventSub notifications via Websockets.
        /// </summary>
        /// <param name="loggerFactory">LoggerFactory used to construct Loggers for the EventSubWebsocketClient and underlying classes</param>
        public EventSubWebsocketClient(ILoggerFactory? loggerFactory = null)
        {
            _loggerFactory = loggerFactory ?? NullLoggerFactory.Instance;

            _logger = _loggerFactory.CreateLogger<EventSubWebsocketClient>();
            _websocketClient = new WebsocketClient(_loggerFactory.CreateLogger<WebsocketClient>());

            _websocketClient.OnDataReceived += OnDataReceived;
            _websocketClient.OnErrorOccurred += OnErrorOccurred;

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
                    : new WebsocketClient(_loggerFactory.CreateLogger<WebsocketClient>());

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

                        await WebsocketReconnected.InvokeAsync(this, new());

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
                : new WebsocketClient(_loggerFactory.CreateLogger<WebsocketClient>());

            _websocketClient.OnDataReceived += OnDataReceived;
            _websocketClient.OnErrorOccurred += OnErrorOccurred;

            if (!await ConnectAsync())
                return false;

            await WebsocketReconnected.InvokeAsync(this, new());

            return true;
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

            await WebsocketDisconnected.InvokeAsync(this, new());
        }

        /// <summary>
        /// AsyncEventHandler for the DataReceived event from the underlying websocket. This is where every notification that gets in gets handled"/>
        /// </summary>
        /// <param name="sender">Sender of the event. In this case <see cref="WebsocketClient"/></param>
        /// <param name="e">EventArgs send with the event. <see cref="DataReceivedArgs"/></param>
        private async Task OnDataReceived(object? sender, DataReceivedArgs e)
        {
            _logger?.LogMessage(e.Bytes);
            _lastReceived = DateTimeOffset.Now;

            var json = JsonDocument.Parse(e.Bytes);
            var metadata = json.RootElement.GetProperty("metadata"u8).Deserialize<WebsocketEventSubMetadata>(_jsonSerializerOptions)!;
            var payload = json.RootElement.GetProperty("payload"u8);

            try
            {
                switch (metadata.MessageType)
                {
                    case "session_welcome":
                        await HandleWelcome(metadata, payload);
                        break;
                    case "session_disconnect":
                        await HandleDisconnect(metadata, payload);
                        break;
                    case "session_reconnect":
                        HandleReconnect(metadata, payload);
                        break;
                    case "session_keepalive":
                        HandleKeepAlive(metadata, payload);
                        break;
                    case "notification":
                        await HandleNotificationAsync(metadata, payload);
                        break;
                    case "revocation":
                        await HandleRevocation(metadata, payload);
                        break;
                    default:
                        _logger?.LogUnknownMessageType(metadata.MessageType);
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogExeption("Error while processing EventSub notification", ex);
            }
        }

        /// <summary>
        /// AsyncEventHandler for the ErrorOccurred event from the underlying websocket. This handler only serves as a relay up to the user code"/>
        /// </summary>
        /// <param name="sender">Sender of the event. In this case <see cref="WebsocketClient"/></param>
        /// <param name="e">EventArgs send with the event. <see cref="ErrorOccuredArgs"/></param>
        private async Task OnErrorOccurred(object? sender, ErrorOccuredArgs e)
        {
            await ErrorOccurred.InvokeAsync(this, e);
        }

        /// <summary>
        /// Handles 'session_reconnect' notifications
        /// </summary>
        private void HandleReconnect(WebsocketEventSubMetadata metadata, JsonElement payload)
        {
            _ = metadata;
            _logger?.LogReconnectRequested(SessionId);
            var data = JsonSerializer.Deserialize<EventSubWebsocketSessionInfoPayload>(payload, _jsonSerializerOptions);
            _reconnectRequested = true;

            Task.Run(async () => await ReconnectAsync(new Uri(data?.Session.ReconnectUrl ?? WEBSOCKET_URL)));
        }

        /// <summary>
        /// Handles 'session_welcome' notifications
        /// </summary>
        private async ValueTask HandleWelcome(WebsocketEventSubMetadata metadata, JsonElement payload)
        {
            _ = metadata;
            var data = JsonSerializer.Deserialize<EventSubWebsocketSessionInfoPayload>(payload, _jsonSerializerOptions);

            if (data is null)
                return;

            if (_reconnectRequested)
                _reconnectComplete = true;

            SessionId = data.Session.Id;
            var keepAliveTimeout = data.Session.KeepaliveTimeoutSeconds + data.Session.KeepaliveTimeoutSeconds * 0.2;

            _keepAliveTimeout = TimeSpan.FromSeconds(keepAliveTimeout ?? 10);

            await WebsocketConnected.InvokeAsync(this, new WebsocketConnectedArgs { IsRequestedReconnect = _reconnectRequested });
        }

        /// <summary>
        /// Handles 'session_disconnect' notifications
        /// </summary>
        private async Task HandleDisconnect(WebsocketEventSubMetadata metadata, JsonElement payload)
        {
            _ = metadata;
            var data = JsonSerializer.Deserialize<EventSubWebsocketSessionInfoPayload>(payload, _jsonSerializerOptions);

            if (data != null)
                _logger?.LogForceDisconnected(data.Session.Id, data.Session.DisconnectedAt, data.Session.DisconnectReason);

            await WebsocketDisconnected.InvokeAsync(this, new());
        }

        /// <summary>
        /// Handles 'session_keepalive' notifications
        /// </summary>
        private void HandleKeepAlive(WebsocketEventSubMetadata metadata, JsonElement payload)
        {
            _ = metadata;
            _ = payload;
        }

        /// <summary>
        /// Handles 'notification' notifications
        /// </summary>
        private async Task HandleNotificationAsync(WebsocketEventSubMetadata metadata, JsonElement payload)
        {
            if (!metadata.HasSubscriptionInfo)
            {
                await ErrorOccurred.InvokeAsync(this, new ErrorOccuredArgs { Exception = new ArgumentException("Unable to determine subscription type or subscription version!") });
                return;
            }
            var task = (metadata.SubscriptionType, metadata.SubscriptionVersion) switch
            {
                ("automod.message.hold", "1") => InvokeEventSubEvent<AutomodMessageHoldArgs, AutomodMessageHold>(AutomodMessageHold),
                ("automod.message.hold", "2") => InvokeEventSubEvent<AutomodMessageHoldV2Args, AutomodMessageHoldV2>(AutomodMessageHoldV2),
                ("automod.message.update", "1") => InvokeEventSubEvent<AutomodMessageUpdateArgs, AutomodMessageUpdate>(AutomodMessageUpdate),
                ("automod.message.update", "2") => InvokeEventSubEvent<AutomodMessageUpdateV2Args, AutomodMessageUpdateV2>(AutomodMessageUpdateV2),
                ("automod.settings.update", "1") => InvokeEventSubEvent<AutomodSettingsUpdateArgs, AutomodSettingsUpdate>(AutomodSettingsUpdate),
                ("automod.terms.update", "1") => InvokeEventSubEvent<AutomodTermsUpdateArgs, AutomodTermsUpdate>(AutomodTermsUpdate),
                ("channel.bits.use", "1") => InvokeEventSubEvent<ChannelBitsUseArgs, ChannelBitsUse>(ChannelBitsUse),
                ("channel.update", "2") => InvokeEventSubEvent<ChannelUpdateArgs, ChannelUpdate>(ChannelUpdate),
                ("channel.follow", "2") => InvokeEventSubEvent<ChannelFollowArgs, ChannelFollow>(ChannelFollow),
                ("channel.ad_break.begin", "1") => InvokeEventSubEvent<ChannelAdBreakBeginArgs, ChannelAdBreakBegin>(ChannelAdBreakBegin),
                ("channel.chat.clear", "1") => InvokeEventSubEvent<ChannelChatClearArgs, ChannelChatClear>(ChannelChatClear),
                ("channel.chat.clear_user_messages", "1") => InvokeEventSubEvent<ChannelChatClearUserMessagesArgs, ChannelChatClearUserMessages>(ChannelChatClearUserMessages),
                ("channel.chat.message", "1") => InvokeEventSubEvent<ChannelChatMessageArgs, ChannelChatMessage>(ChannelChatMessage),
                ("channel.chat.message_delete", "1") => InvokeEventSubEvent<ChannelChatMessageDeleteArgs, ChannelChatMessageDelete>(ChannelChatMessageDelete),
                ("channel.chat.notification", "1") => InvokeEventSubEvent<ChannelChatNotificationArgs, ChannelChatNotification>(ChannelChatNotification),
                ("channel.chat_settings.update", "1") => InvokeEventSubEvent<ChannelChatSettingsUpdateArgs, ChannelChatSettingsUpdate>(ChannelChatSettingsUpdate),
                ("channel.chat.user_message_hold", "1") => InvokeEventSubEvent<ChannelChatUserMessageHoldArgs, ChannelChatUserMessageHold>(ChannelChatUserMessageHold),
                ("channel.chat.user_message_update", "1") => InvokeEventSubEvent<ChannelChatUserMessageUpdateArgs, ChannelChatUserMessageUpdate>(ChannelChatUserMessageUpdate),
                ("channel.shared_chat.begin", "1") => InvokeEventSubEvent<ChannelSharedChatSessionBeginArgs, ChannelSharedChatSessionBegin>(ChannelSharedChatSessionBegin),
                ("channel.shared_chat.update", "1") => InvokeEventSubEvent<ChannelSharedChatSessionUpdateArgs, ChannelSharedChatSessionUpdate>(ChannelSharedChatSessionUpdate),
                ("channel.shared_chat.end", "1") => InvokeEventSubEvent<ChannelSharedChatSessionEndArgs, ChannelSharedChatSessionEnd>(ChannelSharedChatSessionEnd),
                ("channel.subscribe", "1") => InvokeEventSubEvent<ChannelSubscribeArgs, ChannelSubscribe>(ChannelSubscribe),
                ("channel.subscription.end", "1") => InvokeEventSubEvent<ChannelSubscriptionEndArgs, ChannelSubscriptionEnd>(ChannelSubscriptionEnd),
                ("channel.subscription.gift", "1") => InvokeEventSubEvent<ChannelSubscriptionGiftArgs, ChannelSubscriptionGift>(ChannelSubscriptionGift),
                ("channel.subscription.message", "1") => InvokeEventSubEvent<ChannelSubscriptionMessageArgs, ChannelSubscriptionMessage>(ChannelSubscriptionMessage),
                ("channel.cheer", "1") => InvokeEventSubEvent<ChannelCheerArgs, ChannelCheer>(ChannelCheer),
                ("channel.raid", "1") => InvokeEventSubEvent<ChannelRaidArgs, ChannelRaid>(ChannelRaid),
                ("channel.ban", "1") => InvokeEventSubEvent<ChannelBanArgs, ChannelBan>(ChannelBan),
                ("channel.unban", "1") => InvokeEventSubEvent<ChannelUnbanArgs, ChannelUnban>(ChannelUnban),
                ("channel.unban_request.create", "1") => InvokeEventSubEvent<ChannelUnbanRequestCreateArgs, ChannelUnbanRequestCreate>(ChannelUnbanRequestCreate),
                ("channel.unban_request.resolve", "1") => InvokeEventSubEvent<ChannelUnbanRequestResolveArgs, ChannelUnbanRequestResolve>(ChannelUnbanRequestResolve),
                ("channel.moderate", "1") => InvokeEventSubEvent<ChannelModerateArgs, ChannelModerate>(ChannelModerate),
                ("channel.moderate", "2") => InvokeEventSubEvent<ChannelModerateV2Args, ChannelModerateV2>(ChannelModerateV2),
                ("channel.moderator.add", "1") => InvokeEventSubEvent<ChannelModeratorArgs, ChannelModerator>(ChannelModeratorAdd),
                ("channel.moderator.remove", "1") => InvokeEventSubEvent<ChannelModeratorArgs, ChannelModerator>(ChannelModeratorRemove),
                ("channel.guest_star_session.begin", "beta") => InvokeEventSubEvent<ChannelGuestStarSessionBeginArgs, ChannelGuestStarSessionBegin>(ChannelGuestStarSessionBegin),
                ("channel.guest_star_session.end", "beta") => InvokeEventSubEvent<ChannelGuestStarSessionEndArgs, ChannelGuestStarSessionEnd>(ChannelGuestStarSessionEnd),
                ("channel.guest_star_guest.update", "beta") => InvokeEventSubEvent<ChannelGuestStarGuestUpdateArgs, ChannelGuestStarGuestUpdate>(ChannelGuestStarGuestUpdate),
                ("channel.guest_star_settings.update", "beta") => InvokeEventSubEvent<ChannelGuestStarSettingsUpdateArgs, ChannelGuestStarSettingsUpdate>(ChannelGuestStarSettingsUpdate),
                ("channel.channel_points_automatic_reward_redemption.add", "1") => InvokeEventSubEvent<ChannelPointsAutomaticRewardRedemptionArgs, ChannelPointsAutomaticRewardRedemption>(ChannelPointsAutomaticRewardRedemptionAdd),
                ("channel.channel_points_automatic_reward_redemption.add", "2") => InvokeEventSubEvent<ChannelPointsAutomaticRewardRedemptionAddV2Args, ChannelPointsAutomaticRewardRedemptionV2>(ChannelPointsAutomaticRewardRedemptionAddV2),
                ("channel.channel_points_custom_reward.add", "1") => InvokeEventSubEvent<ChannelPointsCustomRewardArgs, ChannelPointsCustomReward>(ChannelPointsCustomRewardAdd),
                ("channel.channel_points_custom_reward.update", "1") => InvokeEventSubEvent<ChannelPointsCustomRewardArgs, ChannelPointsCustomReward>(ChannelPointsCustomRewardUpdate),
                ("channel.channel_points_custom_reward.remove", "1") => InvokeEventSubEvent<ChannelPointsCustomRewardArgs, ChannelPointsCustomReward>(ChannelPointsCustomRewardRemove),
                ("channel.channel_points_custom_reward_redemption.add", "1") => InvokeEventSubEvent<ChannelPointsCustomRewardRedemptionArgs, ChannelPointsCustomRewardRedemption>(ChannelPointsCustomRewardRedemptionAdd),
                ("channel.channel_points_custom_reward_redemption.update", "1") => InvokeEventSubEvent<ChannelPointsCustomRewardRedemptionArgs, ChannelPointsCustomRewardRedemption>(ChannelPointsCustomRewardRedemptionUpdate),
                ("channel.poll.begin", "1") => InvokeEventSubEvent<ChannelPollBeginArgs, ChannelPollBegin>(ChannelPollBegin),
                ("channel.poll.progress", "1") => InvokeEventSubEvent<ChannelPollProgressArgs, ChannelPollProgress>(ChannelPollProgress),
                ("channel.poll.end", "1") => InvokeEventSubEvent<ChannelPollEndArgs, ChannelPollEnd>(ChannelPollEnd),
                ("channel.prediction.begin", "1") => InvokeEventSubEvent<ChannelPredictionBeginArgs, ChannelPredictionBegin>(ChannelPredictionBegin),
                ("channel.prediction.progress", "1") => InvokeEventSubEvent<ChannelPredictionProgressArgs, ChannelPredictionProgress>(ChannelPredictionProgress),
                ("channel.prediction.lock", "1") => InvokeEventSubEvent<ChannelPredictionLockArgs, ChannelPredictionLock>(ChannelPredictionLock),
                ("channel.prediction.end", "1") => InvokeEventSubEvent<ChannelPredictionEndArgs, ChannelPredictionEnd>(ChannelPredictionEnd),
                ("channel.suspicious_user.message", "1") => InvokeEventSubEvent<ChannelSuspiciousUserMessageArgs, ChannelSuspiciousUserMessage>(ChannelSuspiciousUserMessage),
                ("channel.suspicious_user.update", "1") => InvokeEventSubEvent<ChannelSuspiciousUserUpdateArgs, ChannelSuspiciousUserUpdate>(ChannelSuspiciousUserUpdate),
                ("channel.vip.add", "1") => InvokeEventSubEvent<ChannelVipArgs, ChannelVip>(ChannelVipAdd),
                ("channel.vip.remove", "1") => InvokeEventSubEvent<ChannelVipArgs, ChannelVip>(ChannelVipRemove),
                ("channel.warning.acknowledge", "1") => InvokeEventSubEvent<ChannelWarningAcknowledgeArgs, ChannelWarningAcknowledge>(ChannelWarningAcknowledge),
                ("channel.warning.send", "1") => InvokeEventSubEvent<ChannelWarningSendArgs, ChannelWarningSend>(ChannelWarningSend),
                ("channel.charity_campaign.donate", "1") => InvokeEventSubEvent<ChannelCharityCampaignDonateArgs, ChannelCharityCampaignDonate>(ChannelCharityCampaignDonate),
                ("channel.charity_campaign.start", "1") => InvokeEventSubEvent<ChannelCharityCampaignStartArgs, ChannelCharityCampaignStart>(ChannelCharityCampaignStart),
                ("channel.charity_campaign.progress", "1") => InvokeEventSubEvent<ChannelCharityCampaignProgressArgs, ChannelCharityCampaignProgress>(ChannelCharityCampaignProgress),
                ("channel.charity_campaign.stop", "1") => InvokeEventSubEvent<ChannelCharityCampaignStopArgs, ChannelCharityCampaignStop>(ChannelCharityCampaignStop),
                ("conduit.shard.disabled", "1") => InvokeEventSubEvent<ConduitShardDisabledArgs, ConduitShardDisabled>(ConduitShardDisabled),
                ("channel.goal.begin", "1") => InvokeEventSubEvent<ChannelGoalBeginArgs, ChannelGoalBegin>(ChannelGoalBegin),
                ("channel.goal.progress", "1") => InvokeEventSubEvent<ChannelGoalProgressArgs, ChannelGoalProgress>(ChannelGoalProgress),
                ("channel.goal.end", "1") => InvokeEventSubEvent<ChannelGoalEndArgs, ChannelGoalEnd>(ChannelGoalEnd),
                ("channel.hype_train.begin", "2") => InvokeEventSubEvent<ChannelHypeTrainBeginV2Args, HypeTrainBeginV2>(ChannelHypeTrainBeginV2),
                ("channel.hype_train.progress", "2") => InvokeEventSubEvent<ChannelHypeTrainProgressV2Args, HypeTrainProgressV2>(ChannelHypeTrainProgressV2),
                ("channel.hype_train.end", "2") => InvokeEventSubEvent<ChannelHypeTrainEndV2Args, HypeTrainEndV2>(ChannelHypeTrainEndV2),
                ("channel.shield_mode.begin", "1") => InvokeEventSubEvent<ChannelShieldModeBeginArgs, ChannelShieldModeBegin>(ChannelShieldModeBegin),
                ("channel.shield_mode.end", "1") => InvokeEventSubEvent<ChannelShieldModeEndArgs, ChannelShieldModeEnd>(ChannelShieldModeEnd),
                ("channel.shoutout.create", "1") => InvokeEventSubEvent<ChannelShoutoutCreateArgs, ChannelShoutoutCreate>(ChannelShoutoutCreate),
                ("channel.shoutout.receive", "1") => InvokeEventSubEvent<ChannelShoutoutReceiveArgs, ChannelShoutoutReceive>(ChannelShoutoutReceive),
                ("stream.online", "1") => InvokeEventSubEvent<StreamOnlineArgs, StreamOnline>(StreamOnline),
                ("stream.offline", "1") => InvokeEventSubEvent<StreamOfflineArgs, StreamOffline>(StreamOffline),
                ("user.update", "1") => InvokeEventSubEvent<UserUpdateArgs, UserUpdate>(UserUpdate),
                ("user.whisper.message", "1") => InvokeEventSubEvent<UserWhisperMessageArgs, UserWhisperMessage>(UserWhisperMessage),
                _ => InvokeEventSubEvent<UnknownEventSubNotificationArgs, JsonElement>(UnknownEventSubNotification),
            };
            await task;

            async Task InvokeEventSubEvent<TEvent, TModel>(AsyncEventHandler<TEvent>? asyncEventHandler)
                where TEvent : TwitchLibEventSubNotificationArgs<TModel>, new()
            {
                var notification = JsonSerializer.Deserialize<EventSubNotificationPayload<TModel>>(payload, _jsonSerializerOptions);
                await asyncEventHandler.InvokeAsync(this, new TEvent { Metadata = metadata, Payload = notification });
            }
        }

        /// <summary>
        /// Handles 'revocation' notifications
        /// </summary>
        private async Task HandleRevocation(WebsocketEventSubMetadata metadata, JsonElement payload)
        {
            var data = JsonSerializer.Deserialize<EventSubNotificationPayload<object?>>(payload, _jsonSerializerOptions);

            if (data is null)
                throw new InvalidOperationException("Parsed JSON cannot be null!");

            await Revocation?.InvokeAsync(this, new RevocationArgs { Metadata = metadata, Payload = data });
        }
    }
}
