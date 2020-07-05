using System;
using System.Text;
using NATS.Client;
using NATS.Client.Rx;
using NATS.Client.Rx.Ops;
using System.Linq;
using StackExchange.Redis;

namespace Subscriber
{
    public class SubscriberService
    {
        public void Run(IConnection connection)
        {
            var greetings = connection.Observe("JobCreated")
                    .Where(m => m.Data?.Any() == true)
                    .Select(m => Encoding.Default.GetString(m.Data));

            greetings.Subscribe(msg =>
            {
                string description = ConnectionMultiplexer.Connect("localhost").GetDatabase().HashGet(msg, "description");
                Console.WriteLine($"id: {msg}; description: {description}");
            });
        }    
    }
}