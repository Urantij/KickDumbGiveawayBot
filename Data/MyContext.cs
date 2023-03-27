using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KickDumbGiveawayBot.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace KickDumbGiveawayBot.Data;

public class MyContext : DbContext
{
#nullable disable
    public DbSet<GiveawayModel> Giveaways { get; set; }
    public DbSet<PosterModel> Posters { get; set; }
    public DbSet<RollModel> Rolls { get; set; }
#nullable restore

    public MyContext(DbContextOptions<MyContext> options)
    : base(options)
    {

    }
}
