using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KickDumbGiveawayBot.Kick.Messages;

public class ChatMessageMessage
{
    public class ChatMessage
    {
        public class RepliedToMessage
        {
            [JsonPropertyName("id")]
            public string Id { get; set; }

            [JsonPropertyName("message")]
            public string Message { get; set; }

            [JsonPropertyName("username")]
            public string Username { get; set; }

            public RepliedToMessage(string id, string message, string username)
            {
                Id = id;
                Message = message;
                Username = username;
            }
        }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("replied_to")]
        public RepliedToMessage RepliedTo { get; set; }

        [JsonPropertyName("chatroom_id")]
        public string ChatroomId { get; set; }

        [JsonPropertyName("role")]
        public string? Role { get; set; }

        [JsonPropertyName("created_at")]
        public long CreatedAt { get; set; }

        [JsonPropertyName("months_subscribed")]
        public int? MonthsSubscribed { get; set; }

        public ChatMessage(string id, string message, string type, RepliedToMessage repliedTo, string chatroomId, string? role, long createdAt, int? monthsSubscribed)
        {
            Id = id;
            Message = message;
            Type = type;
            RepliedTo = repliedTo;
            ChatroomId = chatroomId;
            Role = role;
            CreatedAt = createdAt;
            MonthsSubscribed = monthsSubscribed;
        }
    }

    public class ChatUser
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("role")]
        public string Role { get; set; }

        [JsonPropertyName("isSuperAdmin")]
        public bool IsSuperAdmin { get; set; }

        [JsonPropertyName("verified")]
        public bool Verified { get; set; }

        [JsonPropertyName("is_subscribed")]
        public bool IsSubscribed { get; set; }

        [JsonPropertyName("is_founder")]
        public bool IsFounder { get; set; }

        public ChatUser(long id, string username, string role, bool isSuperAdmin, bool verified, bool isSubscribed, bool isFounder)
        {
            Id = id;
            Username = username;
            Role = role;
            IsSuperAdmin = isSuperAdmin;
            Verified = verified;
            IsSubscribed = isSubscribed;
            IsFounder = isFounder;
        }
    }

    [JsonPropertyName("message")]
    public ChatMessage Message { get; set; }

    [JsonPropertyName("user")]
    public ChatUser User { get; set; }

    public ChatMessageMessage(ChatMessage message, ChatUser user)
    {
        Message = message;
        User = user;
    }
}
