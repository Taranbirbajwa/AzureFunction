using System;
using System.Text;
using System.Text.Json;
using Azure.Storage.Queues.Models;
using GNPMAzureFunctions.Models;
using GNPMAzureFunctions.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GNPMAzureFunctions
{
    public class SendEmail
    {
        private readonly ILogger<SendEmail> _logger;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public SendEmail(ILogger<SendEmail> logger, 
                        IConfiguration configuration, IEmailService emailService)
        {
            _logger = logger;
            _configuration = configuration;
            _emailService = emailService;
        }

        [Function(nameof(SendEmail))]
        public async Task Run([QueueTrigger("emailqueue", Connection = "AzureWebJobsStorage")] string message)
        {
            Mail? objMail = JsonSerializer.Deserialize<Mail>(message);
            var result = await _emailService.SendEmailAsync(objMail!);
            _logger.LogInformation($"C# Queue trigger function processed: {result}");
        }
    }
}
