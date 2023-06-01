using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TwitchLib.EventSub.Websockets.Core.EventArgs;
using TwitchLib.EventSub.Websockets.Core.EventArgs.Channel;

namespace TwitchLib.EventSub.Websockets.Example.NetStandard
{
    public class WebsocketHostedServiceWithoutDI : IHostedService
    {
        private readonly ILogger<WebsocketHostedService> _logger;
        private readonly EventSubWebsocketClient _eventSubWebsocketClient;
        
        public WebsocketHostedServiceWithoutDI(ILogger<WebsocketHostedService> logger, ILoggerFactory loggerFactory)
        {
            _logger = logger;
            _eventSubWebsocketClient = new EventSubWebsocketClient(loggerFactory);
            
            _eventSubWebsocketClient.WebsocketConnected += OnWebsocketConnected;
            _eventSubWebsocketClient.WebsocketDisconnected += OnWebsocketDisconnected;
            _eventSubWebsocketClient.WebsocketReconnected += OnWebsocketReconnected;
            _eventSubWebsocketClient.ErrorOccurred += OnErrorOccurred;

            _eventSubWebsocketClient.ChannelFollow += OnChannelFollow;
        }
        
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _eventSubWebsocketClient.ConnectAsync();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _eventSubWebsocketClient.DisconnectAsync();
        }
        
        private async Task OnErrorOccurred(object sender, ErrorOccuredArgs e)
        {
            _logger.LogError($"Websocket {_eventSubWebsocketClient.SessionId} - Error occurred!");
        }

        private async Task OnChannelFollow(object sender, ChannelFollowArgs e)
        {
            var eventData = e.Notification.Payload.Event;
            _logger.LogInformation($"{eventData.UserName} followed {eventData.BroadcasterUserName} at {eventData.FollowedAt}");
        }
        
        private async Task OnWebsocketConnected(object sender, WebsocketConnectedArgs e)
        {
            _logger.LogInformation($"Websocket {_eventSubWebsocketClient.SessionId} connected!");

            if (!e.IsRequestedReconnect)
            {
                // subscribe to topics
            }
        }

        private async Task OnWebsocketDisconnected(object sender, EventArgs e)
        {
            _logger.LogError($"Websocket {_eventSubWebsocketClient.SessionId} disconnected!");

            // Don't do this in production. You should implement a better reconnect strategy
            while (!await _eventSubWebsocketClient.ReconnectAsync())
            {
                _logger.LogError("Websocket reconnect failed!");
                await Task.Delay(1000);
            }
        }

        private async Task OnWebsocketReconnected(object sender, EventArgs e)
        {
            _logger.LogWarning($"Websocket {_eventSubWebsocketClient.SessionId} reconnected");
        }
    }
}