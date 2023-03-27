using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KickDumbGiveawayBot.Kick.Messages;

public class ConnectionEstablishedMessage
{
    [JsonPropertyName("socket_id")]
    public string SocketId { get; set; }

    [JsonPropertyName("activity_timeout")]
    public int ActivityTimeout { get; set; }

    public ConnectionEstablishedMessage(string socketId, int activityTimeout)
    {
        SocketId = socketId;
        ActivityTimeout = activityTimeout;
    }
}
