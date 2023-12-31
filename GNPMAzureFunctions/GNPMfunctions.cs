using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using GNPMAzureFunctions.Services;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using GNPMAzureFunctions.Models;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections;
using Mail = GNPMAzureFunctions.Models.Mail;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using System.Text;

namespace GNPMAzureFunctions
{
    public class GNPMfunctions
    {
        public const string expired = "Expired";
        private readonly ILogger _logger;
        private readonly IDatabaseService _databaseService;
        private readonly IConfiguration _configuration;
        public GNPMfunctions(ILoggerFactory loggerFactory, 
                             IDatabaseService databaseService, IEmailService emailService,
                             IConfiguration configuration)
        {
            _logger = loggerFactory.CreateLogger<GNPMfunctions>();
            _databaseService = databaseService;
            _configuration = configuration;
        }
       
        [Function("SendRenewAgreementEmailNotification")]
        //0 */5 * * * *
        //0 0 5 * * *
        public async Task RunAsync([TimerTrigger("0 30 7  * * *", RunOnStartup = false)] MyInfo myTimer)
        {
            try
            {

                List<NotificationReciever> emailNotificationReceivers = await GetNotificationReceiversFromDatabase();
                foreach (var user in emailNotificationReceivers)
                {
                    string templatePath = user.status == expired ? "EmailTemplates/expiredAgreement.html" : "EmailTemplates/renewedAgreement.html";
                    string emailTemplate = File.ReadAllText(templatePath);
                    string appUrl = $"{_configuration["AppUrl"]}{user.agreementNumber}";
                    string emailContent = GenerateEmailContent(user, emailTemplate, appUrl);
                    string toMaiAddress = $"{user.createdBy},{user.salesPerson}";
                    string subject = user.status == expired ? _configuration["expiredAgreementSubjectLine"] : _configuration["RenewAgreementSubjectLine"];
                    string fromEmailAddress = _configuration["FromEmailAddress"];
                    Mail objMail = new Mail(fromEmailAddress, toMaiAddress, "", "", "", subject, "", "", "", emailContent);
                    string objMailJsonMessage = JsonSerializer.Serialize(objMail);

                    // Enqueue the JSON message using Azure SDK
                    var connectionString = _configuration["AzureWebJobsStorage"];
                    var queueName = _configuration["QueueName"];
                    var queueClient = new QueueClient(connectionString, queueName);
                    var bytes = Encoding.UTF8.GetBytes(objMailJsonMessage);
                    await queueClient.SendMessageAsync(Convert.ToBase64String(bytes));
                    _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now} successfully");
                }
            }

            catch (Exception ex)
            {
                _logger.LogError($"An error occurred: {ex.Message}");
            }
        }

        private async Task<List<NotificationReciever>> GetNotificationReceiversFromDatabase()
        {
            List<NotificationReciever> emailNotificationReceivers = new List<NotificationReciever>();
            await _databaseService.ExecuteStoredProcedureAsync(async command =>
            {
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        NotificationReciever reciever = new NotificationReciever(
                             reader.GetString(reader.GetOrdinal("agreementNumber")),
                             reader.GetString(reader.GetOrdinal("accountNumber")),
                             reader.GetString(reader.GetOrdinal("companyName")),
                             reader.GetString(reader.GetOrdinal("status")),
                             reader.GetString(reader.GetOrdinal("createdBy")),
                             reader.GetString(reader.GetOrdinal("salesPerson"))
                        );

                        emailNotificationReceivers.Add(reciever);
                    }
                }
            }, "[dbo.GNPM_USP_GetRenewAgreementNotificationList]");

            return emailNotificationReceivers;
        }

        private string GenerateEmailContent(NotificationReciever user, string emailTemplate, string appUrl)
        {
            return emailTemplate
                .Replace("{1}", user.agreementNumber)
                .Replace("{2}", user.accountNumber)
                .Replace("{3}", user.companyName)
                .Replace("{4}", user.status)
                .Replace("{5}", user.salesPerson)
                .Replace("{6}", user.status)
                .Replace("{7}", appUrl);
        }


    }

}

public class MyInfo
{
    public MyScheduleStatus? ScheduleStatus { get; set; }

    public bool IsPastDue { get; set; }
}

public class MyScheduleStatus
{
    public DateTime Last { get; set; }

    public DateTime Next { get; set; }

    public DateTime LastUpdated { get; set; }
}


