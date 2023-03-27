using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KickDumbGiveawayBot.Data.Models;

public class GiveawayModel
{
    [Key]
    public int Id { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public string CodeWord { get; set; }

    public ICollection<PosterModel>? Posters { get; set; }

    public GiveawayModel() { }
    public GiveawayModel(DateTime startDate, string codeWord)
    {
        StartDate = startDate;
        CodeWord = codeWord;
    }
}
