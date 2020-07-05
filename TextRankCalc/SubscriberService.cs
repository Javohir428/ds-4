using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NATS.Client;
using NATS.Client.Rx;
using NATS.Client.Rx.Ops;
using StackExchange.Redis;

namespace TextRankCalc
{
    class SubscriberService
    {
        private readonly ISet<char> vowels;
        private readonly ISet<char> consonants;
        public SubscriberService()
        {
            vowels = new HashSet<char> { 'a', 'A', 'e', 'E', 'i', 'I', 'o', 'O', 'u', 'U'};
            consonants = new HashSet<char> { 'b', 'B', 'c', 'C', 'd', 'D', 'f', 'F', 'g', 'G', 'h', 'H', 'j', 'J', 'k', 'K', 'l', 'L', 'm', 'M', 'n', 'N', 'p', 'P', 'q', 'Q', 'r', 'R', 's', 'S', 't', 'T', 'v', 'V', 'w', 'W', 'x', 'X', 'y', 'Y', 'z', 'Z' };
        }
        public void Run(IConnection connection)
        {
            var greetings = connection.Observe("JobCreated")
                    .Where(m => m.Data?.Any() == true)
                    .Select(m => Encoding.Default.GetString(m.Data));

            greetings.Subscribe(msg =>
            {
                string data = ConnectionMultiplexer.Connect("localhost").GetDatabase().HashGet(msg, "data");
                int vowelsCount = 0;
                int consonantsCount = 0;
                foreach (char ch in data)
                {
                    if (vowels.Contains(ch))
                    {
                        ++vowelsCount;
                    }
                    else if (consonants.Contains(ch))
                    {
                        ++consonantsCount;
                    }
                }

                ConnectionMultiplexer.Connect("localhost").GetDatabase().HashSet(msg, "text_rank", $"{vowelsCount}/{consonantsCount}");
            });
        }
    }
}