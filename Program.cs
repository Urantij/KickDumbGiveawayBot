using KickDumbGiveawayBot.Content;
using KickDumbGiveawayBot.Data;
using KickDumbGiveawayBot.Kick;
using KickDumbGiveawayBot.Randomness;
using KickDumbGiveawayBot.Telega;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace KickDumbGiveawayBot;

public class Program
{
    public static async Task Main(string[] appArgs)
    {
        AppDomain.CurrentDomain.UnhandledException += (sender, ex) =>
        {
            File.WriteAllText($"CRASH {DateTime.Now:yyyy.MM.dd HH:mm:ss}.txt", ex.ExceptionObject.ToString());
        };
        AppDomain.CurrentDomain.ProcessExit += (sender, e) =>
        {
            SqliteConnection.ClearAllPools();
        };
        AppDomain.CurrentDomain.DomainUnload += (sender, e) =>
        {
            SqliteConnection.ClearAllPools();
        };

        IHost host = Host.CreateDefaultBuilder(appArgs)
        .ConfigureServices((ctx, services) =>
        {
            services.AddDbContextFactory<MyContext>(options =>
                options.UseSqlite(ctx.Configuration.GetConnectionString("Context")));

            services.AddOptions<AppOptions>()
            .BindConfiguration(AppOptions.Key)
            .ValidateDataAnnotations()
            .ValidateOnStart();

            services.AddOptions<TelegaOptions>()
            .BindConfiguration(TelegaOptions.Key)
            .ValidateDataAnnotations()
            .ValidateOnStart();

            services.AddHttpClient();

            services.AddSingleton<Database>();
            services.AddSingleton<MyRandomService>();
            services.AddSingleton<ContentService>();
            services.AddSingleton<KickService>();
            services.AddSingleton<TelegaService>();
        })
        .ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddSimpleConsole(c => c.TimestampFormat = "[HH:mm:ss] ");

#if DEBUG
            {
                logging.SetMinimumLevel(LogLevel.Debug);
            }
#else
            {
                if (appArgs.Contains("--debug"))
                {
                    logging.SetMinimumLevel(LogLevel.Debug);
                }
            }
#endif
        })
        .Build();

        await host.Services.GetRequiredService<Database>().InitAsync();
        await host.Services.GetRequiredService<ContentService>().InitAsync();
        await host.Services.GetRequiredService<KickService>().InitAsync();
        host.Services.GetRequiredService<TelegaService>().Start();

        await host.RunAsync();
    }
}