using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KickDumbGiveawayBot.Kick.Messages;

public class SubscribeMessage
{
    [JsonPropertyName("auth")]
    public string Auth { get; set; }

    [JsonPropertyName("channel")]
    public string Channel { get; set; }

    public SubscribeMessage(string auth, string channel)
    {
        Auth = auth;
        Channel = channel;
    }
}
