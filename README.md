# TwitchLib.EventSub.Websockets
TwitchLib component to connect to Twitch's EventSub service via Websockets also known as EventSockets

## Disclaimer
EventSub via Websockets is still in open beta.
You can use it in production but Twitch may introduce breaking changes without prior notice.
The same goes for this implementation until it reaches Version 1.0.0

## Resources
If you need help on how to setup Dependency Injection in your Console or WPF Application you can have a look at these guides:
- Console: https://dfederm.com/building-a-console-app-with-.net-generic-host/
- WPF: https://laurentkempe.com/2019/09/03/WPF-and-dotnet-Generic-Host-with-dotnet-Core-3-0/

You can also find a console app example for .NET 8 and for .NET Framework 4.8 in the repo.

## Installation

| NuGet            |       | [![TwitchLib.EventSub.Websockets][1]][2]                                       |
| :--------------- | ----: | :--------------------------------------------------------------------------- |
| Package Manager  | `PM>` | `Install-Package TwitchLib.EventSub.Websockets -Version 0.4.0`                 |
| .NET CLI         | `>`   | `dotnet add package TwitchLib.EventSub.Websockets --version 0.4.0`             |
| PackageReference |       | `<PackageReference Include="TwitchLib.EventSub.Websockets" Version="0.4.0" />` |
| Paket CLI        | `>`   | `paket add TwitchLib.EventSub.Websockets --version 0.4.0`                      |

[1]: https://img.shields.io/nuget/v/TwitchLib.EventSub.Websockets.svg?label=TwitchLib.EventSub.Websockets
[2]: https://www.nuget.org/packages/TwitchLib.EventSub.Websockets

## Setup

Step 1: Create a new project (Console, WPF, ASP.NET)

Step 2: Install the TwitchLib.EventSub.Websockets nuget package. (See above on how to do that)

Step 3: Step 3: Add necessary services and config to the DI Container

```csharp
services.AddTwitchLibEventSubWebsockets();
services.AddHostedService<WebsocketHostedService>();
```
(The location of where to put this and the naming of variables might differ depending on what kind of project and general setup you have)

Step 4: Create the HostedService we just added to the DI container and connect to EventSub and listen to Events

```csharp
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using TwitchLib.EventSub.Websockets.Core.EventArgs;
using TwitchLib.EventSub.Websockets.Core.EventArgs.Channel;

namespace TwitchLib.EventSub.Websockets.Test
{
    public class WebsocketHostedService : IHostedService
    {
        private readonly ILogger<WebsocketHostedService> _logger;
        private readonly EventSubWebsocketClient _eventSubWebsocketClient;
        private readonly TwitchApi _twitchApi = new();
        private readonly string _userId;

        public WebsocketHostedService(ILogger<WebsocketHostedService> logger, EventSubWebsocketClient eventSubWebsocketClient)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _eventSubWebsocketClient = eventSubWebsocketClient ?? throw new ArgumentNullException(nameof(eventSubWebsocketClient));
            _eventSubWebsocketClient.WebsocketConnected += OnWebsocketConnected;
            _eventSubWebsocketClient.WebsocketDisconnected += OnWebsocketDisconnected;
            _eventSubWebsocketClient.WebsocketReconnected += OnWebsocketReconnected;
            _eventSubWebsocketClient.ErrorOccurred += OnErrorOccurred;

            _eventSubWebsocketClient.ChannelFollow += OnChannelFollow;
            
            // Get ClientId and ClientSecret by register an Application here: https://dev.twitch.tv/console/apps
            // https://dev.twitch.tv/docs/authentication/register-app/
            _twitchApi.Settings.ClientId = "YOUR_APP_CLIENT_ID";
            // Get Application Token with Client credentials grant flow.
            // https://dev.twitch.tv/docs/authentication/getting-tokens-oauth/#client-credentials-grant-flow
            _twitchApi.Settings.AccessToken = "YOUR_APPLICATION_ACCESS_TOKEN";
            
            // You need the UserID for the User/Channel you want to get Events from.
            // You can use await api.Helix.Users.GetUsersAsync() for that.
            _userId = "USER_ID";
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _eventSubWebsocketClient.ConnectAsync();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _eventSubWebsocketClient.DisconnectAsync();
        }

        private void OnWebsocketConnected(object? sender, WebsocketConnectedArgs e)
        {
            _logger.LogInformation($"Websocket {_eventSubWebsocketClient.SessionId} connected!");

            if (!e.IsRequestedReconnect)
            {
                // subscribe to topics
                
                // create condition Dictionary
                // You need BOTH broadcaster and moderator values or EventSub returns an Error!
                var condition = new Dictionary<string, string> { { "broadcaster_user_id", userId }, {"moderator_user_id", userId} };
                // Create and send EventSubscription
                await _twitchApi.Helix.EventSub.CreateEventSubSubscriptionAsync("channel.follow", "2", condition, EventSubTransportMethod.Websocket,
                _eventSubWebsocketClient.SessionId, accessToken: "BROADCASTER_ACCESS_TOKEN_WITH_SCOPES");
                // If you want to get Events for special Events you need to additionally add the AccessToken of the ChannelOwner to the request.
                // https://dev.twitch.tv/docs/eventsub/eventsub-subscription-types/
            }
         }

        private async void OnWebsocketDisconnected(object? sender, EventArgs e)
        {
            _logger.LogError($"Websocket {_eventSubWebsocketClient.SessionId} disconnected!");

            // Don't do this in production. You should implement a better reconnect strategy with exponential backoff
            while (!await _eventSubWebsocketClient.ReconnectAsync())
            {
                _logger.LogError("Websocket reconnect failed!");
                await Task.Delay(1000);
            }
        }

        private void OnWebsocketReconnected(object? sender, EventArgs e)
        {
            _logger.LogWarning($"Websocket {_eventSubWebsocketClient.SessionId} reconnected");
        }      
      
         private void OnErrorOccurred(object? sender, ErrorOccuredArgs e)
        {
            _logger.LogError($"Websocket {_eventSubWebsocketClient.SessionId} - Error occurred!");
        }

        private void OnChannelFollow(object? sender, ChannelFollowArgs e)
        {
            var eventData = e.Notification.Payload.Event;
            _logger.LogInformation($"{eventData.UserName} followed {eventData.BroadcasterUserName} at {eventData.FollowedAt}");
        }
    }
}
```

Alternatively you can also just clone the examples:
- .NET Framework 4.8 https://github.com/TwitchLib/TwitchLib.EventSub.Websockets/tree/main/TwitchLib.EventSub.Websockets.Example.NetStandard
- .NET 8 -> https://github.com/TwitchLib/TwitchLib.EventSub.Websockets/tree/main/TwitchLib.EventSub.Websockets.Example
