using GNPMAzureFunctions.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults(workerBuilder =>
    {
      workerBuilder.Services.AddTransient<IDatabaseService, DatabaseService>();
      workerBuilder.Services.AddSingleton<IEmailService, EmailService>();
    })
    .ConfigureServices(services =>
    {
       services.AddHttpClient();
    })
    .Build();
host.Run();