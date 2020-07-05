using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using NATS.Client;
using StackExchange.Redis;

namespace BackendApi.Services
{
    public class JobService : Job.JobBase
    {
        private readonly static IConnection connection = new ConnectionFactory().CreateConnection("nats://127.0.0.1");
        private readonly static GreeterService greeter = new GreeterService();

        public override Task<RegisterResponse> Register(RegisterRequest request, ServerCallContext context)
        {
            string id = Guid.NewGuid().ToString();

            ConnectionMultiplexer.Connect("localhost").GetDatabase().HashSet(id, "description", request.Description);
            ConnectionMultiplexer.Connect("localhost").GetDatabase().HashSet(id, "data", request.Data);

            greeter.Run(connection, id);

            var resp = new RegisterResponse
            {
                Id = id
            };

            return Task.FromResult(resp);
        }

        public override Task<ProcessingResult> GetProcessingResult(RegisterResponse jobId, ServerCallContext context)
        {
            var result = new ProcessingResult
            {
                ProcessingComplete = false,
                TextRank = ""
            };

            for (int i = 0; i < 8; ++i)
            {
                Thread.Sleep(2000);
                string textRank = ConnectionMultiplexer.Connect("localhost").GetDatabase().HashGet(jobId.Id, "text_rank");
                if (textRank != null)
                {
                    result.ProcessingComplete = true;
                    result.TextRank = textRank;
                    break;
                }
            }

            return Task.FromResult(result);
        }
    }
}