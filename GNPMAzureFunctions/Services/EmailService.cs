using GNPMAzureFunctions.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GNPMAzureFunctions.Services
{
   

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        public EmailService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClient = httpClientFactory.CreateClient();

        }
        public  async Task<string> SendEmailAsync(Mail objMail)
        {
            string KeyHeader = _configuration["KeyHeaderName"];
            string key = _configuration["SubscriptionKey"];
            string url = _configuration["MailNotificationAPI"];
            var json = JsonSerializer.Serialize(objMail);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            _httpClient.DefaultRequestHeaders.Add(KeyHeader, key);
            var response = await _httpClient.PostAsync(url, data);
            var result = await response.Content.ReadAsStringAsync();
            return result;

        }
    }
}
