using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KickDumbGiveawayBot.Data;
using KickDumbGiveawayBot.Data.Models;
using KickDumbGiveawayBot.Randomness;
using Microsoft.Extensions.Options;

namespace KickDumbGiveawayBot.Content;

/// <summary>
/// Проблема поточности, что тут гивевей как мыло - может выскользнуть в любой момент.
/// Я просто сделаю вид, что нет.
/// </summary>
public class ContentService
{
    private readonly MyRandomService _myRandom;
    private readonly Database _database;
    private readonly ILogger<ContentService> _logger;

    public Giveaway? Giveaway { get; private set; }

    readonly bool forceRandomOrg;

    public ContentService(MyRandomService myRandom, Database database, ILogger<ContentService> logger, IOptions<AppOptions> options)
    {
        this._myRandom = myRandom;
        this._database = database;
        this._logger = logger;

        forceRandomOrg = options.Value.ForceRandomOrg;
    }

    public async Task InitAsync()
    {
        var giveawayModel = await _database.LoadLastGiveaway();
        if (giveawayModel == null)
            return;

        Giveaway = new Giveaway(giveawayModel.Id, giveawayModel.CodeWord);
        if (giveawayModel.Posters == null)
            return;

        foreach (var userModel in giveawayModel.Posters)
        {
            var user = Giveaway.AddUser(userModel.KickId, userModel.Username, false);
            if (user == null)
                continue;

            user.dbId = userModel.Id;
        }
    }

    public async Task CreateNewGiveawayAsync(string codeWord)
    {
        GiveawayModel giveawayModel = new(DateTime.UtcNow, codeWord);

        await _database.AddGiveawayAsync(giveawayModel);

        Giveaway = new Giveaway(giveawayModel.Id, giveawayModel.CodeWord);
    }

    public async Task AddUserAsync(string username, long userId, bool special)
    {
        if (Giveaway == null)
            throw new NullReferenceException(nameof(Giveaway));

        if (Giveaway.TryUpdateUser(userId))
            return;

        var user = Giveaway.AddUser(userId, username, special);
        if (user == null)
            return;

        PosterModel posterModel = new(Giveaway.dbId, userId, username, DateTime.UtcNow);
        await _database.AddPosterAsync(posterModel);

        user.dbId = posterModel.Id;
    }

    /// <summary>
    /// Кидает ошибку, если рендоморг не смог и он зафоршен.
    /// </summary>
    /// <returns></returns>
    public async Task<User?> RollAsync()
    {
        if (Giveaway == null)
            return null;

        int count = Giveaway.GetCount();

        if (count < 2)
            return null;

        int index = await _myRandom.GetIntAsync(0, count - 1, forceRandomOrg);
        User user = Giveaway.GetUser(index);

        RollModel rollModel = new(user.dbId, DateTime.UtcNow);
        await _database.AddRollAsync(rollModel);

        return user;
    }
}
