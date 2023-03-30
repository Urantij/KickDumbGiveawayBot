using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KickDumbGiveawayBot.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace KickDumbGiveawayBot.Data;

public class Database
{
    private readonly IDbContextFactory<MyContext> _contextFactory;

    public Database(IDbContextFactory<MyContext> contextFactory)
    {
        this._contextFactory = contextFactory;
    }

    public async Task InitAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        await context.Database.MigrateAsync();
    }

    public async Task<GiveawayModel?> LoadLastGiveaway()
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        return await context.Giveaways
        .OrderByDescending(g => g.Id)
        .Include(g => g.Posters)
        .FirstOrDefaultAsync();
    }

    public async Task AddGiveawayAsync(GiveawayModel giveaway)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        context.Giveaways.Add(giveaway);

        await context.SaveChangesAsync();
    }

    public async Task AddPosterAsync(PosterModel poster)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        context.Posters.Add(poster);

        await context.SaveChangesAsync();
    }

    public async Task AddRollAsync(RollModel roll)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        context.Rolls.Add(roll);

        await context.SaveChangesAsync();
    }
}
