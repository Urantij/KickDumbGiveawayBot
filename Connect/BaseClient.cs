using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace KickDumbGiveawayBot.Connect;

// https://github.com/Urantij/TwitchSimpleLib/blob/master/Irc/BaseClient.cs

public class BaseClient
{
    public bool IsConnected => connection?.Connected == true;

    public bool Closed { get; private set; }

    public event Action? Connected;
    public event Action<Exception?>? ConnectionClosed;
    public event Action<(Exception exception, string message)>? MessageProcessingException;

    protected readonly ReconnectionTime reconnectionTime;

    protected ILoggerFactory? _loggerFactory;
    protected ILogger? _logger;

    private readonly Uri uri;
    private readonly TimeSpan connectionTimeout;
    protected WsConnection? connection;

    protected BaseClient(Uri uri, ILoggerFactory? loggerFactory)
    {
        this.uri = uri;
        this.connectionTimeout = TimeSpan.FromSeconds(10.0);
        this._loggerFactory = loggerFactory;
        this._logger = loggerFactory?.CreateLogger(this.GetType());

        this.reconnectionTime = new ReconnectionTime(TimeSpan.FromMilliseconds(500.0), TimeSpan.FromSeconds(15.0));
    }

    public async Task ConnectAsync()
    {
        Closed = false;

        WsConnection caller = connection = new WsConnection(uri, _loggerFactory);
        caller.MessageReceived += OnMessageReceived;
        caller.Disposing += ConnectionDisposing;

        if (await caller.StartAsync(connectionTimeout))
        {
            reconnectionTime.Connected();

            await ConnectedAsync(caller);

            Connected?.Invoke();
        }
    }

    public Task SendRawAsync(string content)
        => SendRawAsync(connection, content);

    public static Task SendRawAsync(WsConnection? connection, string content)
    {
        if (connection != null)
            return connection.SendAsync(content);

        return Task.CompletedTask;
    }

    public static void SendRaw(WsConnection? connection, string content)
    {
        connection?.Send(content);
    }

    public void Close()
    {
        Closed = true;
        connection?.Dispose();
    }

    protected virtual Task ConnectedAsync(WsConnection connection)
    {
        return Task.CompletedTask;
    }

    private void OnMessageReceived(object? sender, string e)
    {
        try
        {
            MessageReceived(sender, e);
        }
        catch (Exception ex)
        {
            MessageProcessingException?.Invoke((ex, e));
        }
    }

    protected virtual void MessageReceived(object? sender, string e)
    {
    }

    protected virtual void ConnectionDisposing(object? sender, Exception? e)
    {
        if (!Closed)
        {
            Task.Run(async () =>
            {
                TimeSpan waitTime = reconnectionTime.DoAttempt();

                waitTime += TimeSpan.FromMilliseconds(RandomNumberGenerator.GetInt32(50, 750));

                await Task.Delay(waitTime);

                await ConnectAsync();
            });
        }

        ConnectionClosed?.Invoke(e);
    }
}
