using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using KickDumbGiveawayBot.Content;
using KickDumbGiveawayBot.Data.Models;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace KickDumbGiveawayBot.Telega;

public class TelegaService
{
    static readonly Regex idRegex = new(@"(?'id'\d+)", RegexOptions.Compiled);

    private readonly ContentService _content;
    private readonly ILogger _logger;
    private readonly IDisposable? optionsListener;

    long[] allowedPeers;

    readonly TelegramBotClient bot;

    public TelegaService(ContentService content, ILogger<TelegaService> logger, IOptionsMonitor<TelegaOptions> options)
    {
        this._content = content;
        _logger = logger;

        optionsListener = options.OnChange(OptionsChanged);

        OptionsChanged(options.CurrentValue, null);

        bot = new TelegramBotClient(options.CurrentValue.TelegramToken);
    }

    private void OptionsChanged(TelegaOptions options, string? name)
    {
        allowedPeers = idRegex.Matches(options.TelegramIds)
        .Select(m => long.Parse(m.Groups["id"].Value))
        .ToArray();
    }

    public void Start()
    {
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = new UpdateType[] { UpdateType.Message }
        };

        bot.StartReceiving(
            updateHandler: HandleUpdateAsync,
            pollingErrorHandler: HandlePollingErrorAsync,
            receiverOptions: receiverOptions
        );
    }

    private async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
    {
        if (update.Message is not { } message)
            return;

        if (message.Text is not { } messageText)
            return;

        if (!allowedPeers.Contains(message.Chat.Id))
        {
            _logger.LogWarning("Неизвестный тип стучится {id} ({name})", message.Chat.Id, message.Chat.FirstName);
            return;
        }

        string command;
        string[] args;
        {
            var lines = messageText.Split('\n');

            var commandLine = lines[0].Split(' ');

            command = commandLine[0];
            args = commandLine[1..];
        }

        _logger.LogInformation("Команда: {id} ({name}) {command} {args}", message.Chat.Id, message.Chat.FirstName, command, string.Join(" ", args));

        if (command == "старт")
        {
            if (args.Length == 0)
            {
                await bot.SendTextMessageAsync(message.Chat.Id, "старт слово", cancellationToken: cancellationToken);
                return;
            }

            string codeWord = args[0];

            await _content.CreateNewGiveawayAsync(codeWord);

            await bot.SendTextMessageAsync(message.Chat.Id, "Сделано.", cancellationToken: cancellationToken);
        }
        else if (command == "дай")
        {
            var giveaway = _content.Giveaway;

            if (giveaway == null)
            {
                await bot.SendTextMessageAsync(message.Chat.Id, "Розыгрышей не видно.", cancellationToken: cancellationToken);
                return;
            }

            int count = giveaway.GetCount();

            if (count == 0)
            {
                await bot.SendTextMessageAsync(message.Chat.Id, "Нет участников.", cancellationToken: cancellationToken);
                return;
            }
            if (count == 1)
            {
                var single = giveaway.GetUser(0);
                await bot.SendTextMessageAsync(message.Chat.Id, $"Один участник {single.name} ({single.spamCount})", cancellationToken: cancellationToken);
                return;
            }

            Content.User? user;
            try
            {
                user = await _content.RollAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Roll");

                await bot.SendTextMessageAsync(message.Chat.Id, "Ошибка, увы.", cancellationToken: cancellationToken);
                return;
            }

            if (user == null)
            {
                await bot.SendTextMessageAsync(message.Chat.Id, "Никого, увы.", cancellationToken: cancellationToken);
                return;
            }

            if (user.isSpecial)
            {
                await bot.SendTextMessageAsync(message.Chat.Id, $"Зарплату получает {user.name} ({user.spamCount})", cancellationToken: cancellationToken);
            }
            else
            {
                await bot.SendTextMessageAsync(message.Chat.Id, $"Победил {user.name} ({user.spamCount})", cancellationToken: cancellationToken);
            }
        }
        else if (command == "статус")
        {
            var giveaway = _content.Giveaway;

            if (giveaway == null)
            {
                await bot.SendTextMessageAsync(message.Chat.Id, "Розыгрышей не видно.", cancellationToken: cancellationToken);
                return;
            }

            var users = giveaway.GetUsers();

            string cat = string.Join(", ", users.Select(u => u.name));

            await bot.SendTextMessageAsync(message.Chat.Id, $"Всего: {users.Length}.\n{cat}", cancellationToken: cancellationToken);
        }
    }

    private Task HandlePollingErrorAsync(ITelegramBotClient client, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Ошибка в HandlePollingErrorAsync");

        return Task.CompletedTask;
    }
}
