using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KickDumbGiveawayBot.Randomness;

public class RandomOrgModel
{
    public class ResultL
    {
        public class RandomL
        {
            [JsonPropertyName("data")]
            public List<int> Data { get; set; }

            [JsonPropertyName("completionTime")]
            public string CompletionTime { get; set; }

            public RandomL(List<int> data, string completionTime)
            {
                Data = data;
                CompletionTime = completionTime;
            }
        }

        [JsonPropertyName("random")]
        public RandomL Random { get; set; }

        [JsonPropertyName("bitsUsed")]
        public int BitsUsed { get; set; }

        [JsonPropertyName("bitsLeft")]
        public int BitsLeft { get; set; }

        [JsonPropertyName("requestsLeft")]
        public int RequestsLeft { get; set; }

        [JsonPropertyName("advisoryDelay")]
        public int AdvisoryDelay { get; set; }

        public ResultL(RandomL random, int bitsUsed, int bitsLeft, int requestsLeft, int advisoryDelay)
        {
            Random = random;
            BitsUsed = bitsUsed;
            BitsLeft = bitsLeft;
            RequestsLeft = requestsLeft;
            AdvisoryDelay = advisoryDelay;
        }
    }

    [JsonPropertyName("result")]
    public ResultL Result { get; set; }

    public RandomOrgModel(ResultL result)
    {
        Result = result;
    }
}
