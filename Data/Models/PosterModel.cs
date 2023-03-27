using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KickDumbGiveawayBot.Data.Models;

public class PosterModel
{
    [Key]
    public int Id { get; set; }

    [ForeignKey(nameof(Giveaway))]
    public int GiveawayId { get; set; }
    public GiveawayModel Giveaway { get; set; }

    [Required]
    public long KickId { get; set; }
    [Required]
    public string Username { get; set; }

    [Required]
    public DateTime FirstDate { get; set; }

    public ICollection<RollModel>? Rolls { get; set; }

    public PosterModel() { }
    public PosterModel(int giveawayId, long kickId, string username, DateTime firstDate)
    {
        GiveawayId = giveawayId;
        KickId = kickId;
        Username = username;
        FirstDate = firstDate;
    }
}
