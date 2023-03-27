using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KickDumbGiveawayBot.Data.Models;

public class RollModel
{
    [Key]
    public int Id { get; set; }

    [ForeignKey(nameof(Poster))]
    public int PosterId { get; set; }
    public PosterModel Poster { get; set; }

    [Required]
    public DateTime Date { get; set; }

    public RollModel() { }
    public RollModel(int posterId, DateTime date)
    {
        PosterId = posterId;
        Date = date;
    }
}
