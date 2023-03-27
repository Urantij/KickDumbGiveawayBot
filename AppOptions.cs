using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KickDumbGiveawayBot;

public class AppOptions
{
    public const string Key = "Options";

    [Required]
    public required string ChatRoomId { get; set; }

    [Required]
    public required Uri KickWsUri { get; set; }

    public string? RandomOrgApiKey { get; set; }

    public bool ForceRandomOrg { get; set; }
}
