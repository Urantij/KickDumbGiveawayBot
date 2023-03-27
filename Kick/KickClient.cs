using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using KickDumbGiveawayBot.Connect;
using KickDumbGiveawayBot.Kick.Messages;

namespace KickDumbGiveawayBot.Kick;

public class KickClient : BaseClient
{
    private readonly string chatRoomId;

    private PingManager? pingManager;

    public event Action<ChatMessageMessage>? ChatMessageReceived;

    public KickClient(string chatRoomId, Uri uri, ILoggerFactory? loggerFactory)
        : base(uri, loggerFactory)
    {
        this.chatRoomId = chatRoomId;
    }

    async Task SendMessageAsync(WsConnection? connection, string eventName, string channel, object data)
    {
        var content = JsonSerializer.Serialize(new
        {
#pragma warning disable IDE0037
            channel = channel,
            data = data,
            @event = eventName,
#pragma warning restore IDE0037
        });

        await SendRawAsync(connection, content);
    }

    async Task SendMessageAsync(WsConnection? connection, string eventName, object data)
    {
        var content = JsonSerializer.Serialize(new
        {
#pragma warning disable IDE0037
            data = data,
            @event = eventName,
#pragma warning restore IDE0037
        });

        await SendRawAsync(connection, content);
    }

    void SendMessage(WsConnection? connection, string eventName, object data)
    {
        var content = JsonSerializer.Serialize(new
        {
#pragma warning disable IDE0037
            data = data,
            @event = eventName,
#pragma warning restore IDE0037
        });

        SendRaw(connection, content);
    }

    protected override Task ConnectedAsync(WsConnection connection)
    {
        pingManager = new(false, TimeSpan.FromMinutes(1), TimeSpan.FromSeconds(10));
        pingManager.Pinging += Pinging;
        pingManager.Timeouted += Timeouted;
        pingManager.Start();

        return base.ConnectedAsync(connection);
    }

    protected override void MessageReceived(object? sender, string text)
    {
        var connection = (WsConnection)sender!;

        var doc = JsonDocument.Parse(text);

        string? eventName;
        string? dataContent;
        try
        {
            eventName = doc.RootElement.GetProperty("event").GetString();
            dataContent = doc.RootElement.GetProperty("data").GetString();

            if (eventName == null)
                throw new NullReferenceException($"{nameof(eventName)}");
            if (dataContent == null)
                throw new NullReferenceException($"{nameof(dataContent)}");
        }
        catch (Exception ex)
        {
            _logger?.LogCritical(ex, "Непонятное сообщение. {text}", text);
            return;
        }

        string unescapedMessage = dataContent.Replace("\\\"", "\"");

        if (eventName == "pusher:connection_established")
        {
            var content = JsonSerializer.Deserialize<ConnectionEstablishedMessage>(unescapedMessage);

            if (content == null)
            {
                _logger?.LogCritical("Непонятное сообщение connection_established. {text}", text);
                return;
            }

            OnConnectionEstablished(connection);
        }
        else if (eventName == "pusher_internal:subscription_succeeded")
        {
            string? channel;
            try
            {
                channel = doc.RootElement.GetProperty("channel").GetString();

                if (channel == null)
                    throw new NullReferenceException($"{nameof(channel)}");
            }
            catch (Exception ex)
            {
                _logger?.LogCritical(ex, "Непонятное сообщение subscription_succeeded. {text}", text);
                return;
            }

            _logger?.LogInformation("Подписка сработала {channel}", channel);
        }
        else if (eventName == "pusher:pong")
        {
            pingManager?.PongReceived("");
        }
        else if (eventName == @"App\Events\ChatMessageSentEvent")
        {
            var content = JsonSerializer.Deserialize<ChatMessageMessage>(unescapedMessage);

            if (content == null)
            {
                _logger?.LogCritical("Непонятное сообщение ChatMessageSentEvent. {text}", text);
                return;
            }

            OnChatMessage(content);
        }
    }

    void OnConnectionEstablished(WsConnection connection)
    {
        SubscribeMessage subscribeChatRoomsMessage = new("", $"chatrooms.{chatRoomId}");

        SendMessage(connection, "pusher:subscribe", subscribeChatRoomsMessage);
    }

    private void OnChatMessage(ChatMessageMessage content)
    {
        ChatMessageReceived?.Invoke(content);
    }

    private async Task Pinging(PingManager pingManager, string arg2)
    {
        if (pingManager != this.pingManager)
            return;

        await SendMessageAsync(connection, "pusher:ping", new
        {

        });
    }

    private void Timeouted(PingManager pingManager)
    {
        if (pingManager != this.pingManager)
            return;

        connection!.Dispose(new Exception("Ping Timeout"));
    }

    protected override void ConnectionDisposing(object? sender, Exception? e)
    {
        if (pingManager != null)
        {
            pingManager.Pinging -= Pinging;
            pingManager.Timeouted -= Timeouted;
            pingManager.Stop();
        }

        base.ConnectionDisposing(sender, e);
    }
}
