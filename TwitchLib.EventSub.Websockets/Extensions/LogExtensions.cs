using System;
using System.Net.WebSockets;
using System.Text;
using Microsoft.Extensions.Logging;
using TwitchLib.EventSub.Websockets.Client;

namespace TwitchLib.EventSub.Websockets.Extensions
{
    internal static partial class LogExtensions
    {
        const LogLevel LogMessageLogLevel = LogLevel.Debug;

        [LoggerMessage(LogLevel.Error, "Exeption was throw when raising '{eventName}' event.")]
        public static partial void LogRaiseEventExeption(this ILogger<EventSubWebsocketClient> logger, string eventName, Exception ex);

        [LoggerMessage(LogMessageLogLevel, "{message}")]
        public static partial void LogMessage(this ILogger<EventSubWebsocketClient> logger, string message);

        [LoggerMessage(LogLevel.Critical, "Websocket {sessionId} disconnected at {disconnectedAt}. Reason: {disconnectReason}")]
        public static partial void LogForceDisconnected(this ILogger<EventSubWebsocketClient> logger, string sessionId, DateTime? disconnectedAt, string disconnectReason);
        
        [LoggerMessage(LogLevel.Warning, "Websocket reconnect for SessionId {sessionId} requested!")]
        public static partial void LogReconnectRequested(this ILogger<EventSubWebsocketClient> logger, string sessionId);
        
        [LoggerMessage(LogLevel.Error, "Websocket reconnect for SessionId {sessionId} failed!")]
        public static partial void LogReconnectFailed(this ILogger<EventSubWebsocketClient> logger, string sessionId);
        
        [LoggerMessage(LogLevel.Warning, "Found unknown message type: {messageType}")]
        public static partial void LogUnknownMessageType(this ILogger<EventSubWebsocketClient> logger, string messageType);
        
        [LoggerMessage(LogLevel.Critical, "{closeStatus} - {closeStatusDescription}")]
        public static partial void LogWebsocketClosed(this ILogger<WebsocketClient> logger, WebSocketCloseStatus closeStatus, string closeStatusDescription);

        public static void LogMessage(this ILogger<EventSubWebsocketClient> logger, byte[] message)
        {
            if (logger.IsEnabled(LogMessageLogLevel))
            {
                __LogMessageCallback(logger, Encoding.UTF8.GetString(message), null);
            }
        }
    }
}
