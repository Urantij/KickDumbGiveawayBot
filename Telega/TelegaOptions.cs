using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KickDumbGiveawayBot.Telega;

public class TelegaOptions
{
    public const string Key = "Telegram";

    [Required]
    public required string TelegramIds { get; set; }

    [Required]
    public required string TelegramToken { get; set; }
}
