﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Frontend.Models;
using BackendApi;
using Grpc.Net.Client;

namespace Frontend.Controllers
{
    public class Riderct
    {
        public string  Resp { get; set; }
    }
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetTask(string taskDescription, string data)
        {
            if (taskDescription == null)
            {
                return View("Error", new ErrorViewModel {RequestId = "No Task"});
            }

            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            using var channel = GrpcChannel.ForAddress("http://localhost:5000");
            var client = new Job.JobClient(channel);
            RegisterResponse reply = await client.RegisterAsync(
                new RegisterRequest {Description = taskDescription, Data = data});

            Riderct id = new Riderct{ Resp = reply.Id};
        
            return RedirectToAction("TextDetails", id);
        }
        
        [HttpGet] 
        public IActionResult TextDetails(Riderct id)
        {
            RegisterResponse reply  = new RegisterResponse{ Id = id.Resp};
            

            using var channel = GrpcChannel.ForAddress("http://localhost:5000");
            var client = new Job.JobClient(channel);
            var repl = client.GetProcessingResult(reply);
            
            return View("GetTask", repl);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
