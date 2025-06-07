using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TwitchLib.EventSub.Core;
using TwitchLib.EventSub.Websockets.Core.EventArgs;
using TwitchLib.EventSub.Websockets.Extensions;

#if NET6_0_OR_GREATER
using System.Buffers;
#endif

namespace TwitchLib.EventSub.Websockets.Client
{
    /// <summary>
    /// Websocket client to connect to variable websocket servers
    /// </summary>
    public class WebsocketClient : IDisposable
    {
        /// <summary>
        /// Determines if the Client is still connected based on WebsocketState
        /// </summary>
        public bool IsConnected => _webSocket.State == WebSocketState.Open;
        /// <summary>
        /// Determines if the Client has encountered an unrecoverable issue based on WebsocketState
        /// </summary>
        public bool IsFaulted => _webSocket.CloseStatus != WebSocketCloseStatus.Empty && _webSocket.CloseStatus != WebSocketCloseStatus.NormalClosure;

        internal event AsyncEventHandler<DataReceivedArgs>? OnDataReceived;
        internal event AsyncEventHandler<ErrorOccuredArgs>? OnErrorOccurred;

        private ClientWebSocket _webSocket;
        private readonly ILogger<WebsocketClient>? _logger;

        /// <summary>
        /// Constructor to create a new Websocket client with a logger
        /// </summary>
        /// <param name="logger">Logger used by the websocket client to print various state info</param>
        public WebsocketClient(ILogger<WebsocketClient>? logger = null)
        {
            _webSocket = new ClientWebSocket();
            _logger = logger;
        }

        /// <summary>
        /// Connects the websocket client to a given Websocket Server
        /// </summary>
        /// <param name="url">Websocket Server URL to connect to</param>
        /// <returns>true: if the connection is already open or was successful false: if the connection failed to be established</returns>
        public async Task<bool> ConnectAsync(Uri url)
        {
            try
            {
                if (_webSocket.State is WebSocketState.Open or WebSocketState.Connecting)
                    return true;
                if (_webSocket.State is WebSocketState.Closed)  //after a socken is closed it cannot be reopened
                    _webSocket = new();

                await _webSocket.ConnectAsync(url, CancellationToken.None);

#pragma warning disable 4014
                Task.Run(async () => await ProcessDataAsync());
#pragma warning restore 4014

                return IsConnected;
            }
            catch (Exception ex)
            {
                OnErrorOccurred?.Invoke(this, new ErrorOccuredArgs { Exception = ex });
                return false;
            }
        }

        /// <summary>
        /// Disconnect the Websocket client from its currently connected server
        /// </summary>
        /// <returns>true: if the disconnect was successful without errors false: if the client encountered an issue during the disconnect</returns>
        public async Task<bool> DisconnectAsync()
        {
            try
            {
                if (_webSocket.State is WebSocketState.Open or WebSocketState.Connecting)
                    await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);

                return true;
            }
            catch (Exception ex)
            {
                OnErrorOccurred?.Invoke(this, new ErrorOccuredArgs { Exception = ex });
                return false;
            }
        }

        /// <summary>
        /// Background operation to process incoming data via the websocket
        /// </summary>
        /// <returns>Task representing the background operation</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private async Task ProcessDataAsync()
        {
            const int bufferLength = 4096;
#if NETSTANDARD2_0
            var buffer = new ArraySegment<byte>(new byte[bufferLength]);
#else
            var buffer = new Memory<byte>(new byte[bufferLength]);
#endif
            var store = new byte[4096];
            var payloadSize = 0;

            while (IsConnected)
            {
                try
                {
#if NETSTANDARD2_0
                    WebSocketReceiveResult receiveResult;
#else
                    ValueWebSocketReceiveResult receiveResult;
#endif
                    do
                    {
                        receiveResult = await _webSocket.ReceiveAsync(buffer, CancellationToken.None);

                        if (payloadSize + receiveResult.Count >= store.Length)
                        {
                            var newStoreLength = store.Length + Math.Max(bufferLength, receiveResult.Count);
                            var newStore = new byte[newStoreLength];
                            store.AsSpan().CopyTo(newStore);
                            store = newStore;
                        }

                        buffer
#if NETSTANDARD2_0
                            .Array.AsSpan(0, receiveResult.Count)
#else
                            .Span.Slice(0, receiveResult.Count)
#endif
                            .CopyTo(store.AsSpan(payloadSize));

                        payloadSize += receiveResult.Count;
                    } while (!receiveResult.EndOfMessage);

                    switch (receiveResult.MessageType)
                    {
                        case WebSocketMessageType.Text:
                            if (payloadSize == 0)
                                continue;

                            OnDataReceived?.Invoke(this, new DataReceivedArgs { Bytes = store.AsSpan(0, payloadSize).ToArray() });
                            payloadSize = 0;
                            break;
                        case WebSocketMessageType.Binary:
                            break;
                        case WebSocketMessageType.Close:
                            _logger?.LogWebsocketClosed((WebSocketCloseStatus)_webSocket.CloseStatus!, _webSocket.CloseStatusDescription!);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                catch (Exception ex)
                {
                    OnErrorOccurred?.Invoke(this, new ErrorOccuredArgs { Exception = ex });
                    break;
                }
            }
        }

        /// <summary>
        /// Cleanup of any unused resources as per IDisposable guidelines
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            _webSocket.Dispose();
        }
    }
}