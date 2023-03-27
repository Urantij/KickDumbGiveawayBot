using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KickDumbGiveawayBot.Content;
using KickDumbGiveawayBot.Kick.Messages;
using Microsoft.Extensions.Options;

namespace KickDumbGiveawayBot.Kick;

public class KickService
{
    private readonly ContentService _content;
    private readonly ILogger<KickService> _logger;

    readonly KickClient client;

    public KickService(ContentService content, ILoggerFactory loggerFactory, IOptions<AppOptions> options)
    {
        this._content = content;
        this._logger = loggerFactory.CreateLogger<KickService>();

        client = new(options.Value.ChatRoomId, options.Value.KickWsUri, loggerFactory);
        client.Connected += ClientConnected;
        client.ConnectionClosed += ClientDisconnected;
        client.ChatMessageReceived += ClientMessaged;
        client.MessageProcessingException += ClientProcessingException;
    }

    public async Task InitAsync()
    {
        await client.ConnectAsync();
    }

    private void ClientConnected()
    {
        _logger.LogInformation("Клиент подключился.");
    }

    private void ClientDisconnected(Exception? obj)
    {
        _logger.LogInformation(obj, "Клиент потерял соединение.");
    }

    private async void ClientMessaged(ChatMessageMessage obj)
    {
        if (obj.Message.Message == "тест")
        {
            _logger.LogWarning("Тест! {username}", obj.User.Username);
            return;
        }

        var giveaway = _content.Giveaway;
        if (giveaway == null)
            return;

        if (!giveaway.codeWord.Equals(obj.Message.Message, StringComparison.OrdinalIgnoreCase))
            return;

        bool isSpecial = !string.IsNullOrEmpty(obj.User.Role);

        await _content.AddUserAsync(obj.User.Username, obj.User.Id, isSpecial);
    }

    private void ClientProcessingException((Exception exception, string message) obj)
    {
        _logger.LogCritical(obj.exception, "Ошибка при обработке сообщения:\n{text}", obj.message);
    }
}
