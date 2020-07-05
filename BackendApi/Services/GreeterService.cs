using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NATS.Client;

namespace BackendApi.Services
{
    public class GreeterService
    {
        public void Run(IConnection connection, string id)
        {
            byte[] payload = Encoding.Default.GetBytes(id);
            connection.Publish("JobCreated", payload);
        }
    }
}