using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace OfficeAFunctionApp
{
    public static class OfficeAFunction
    {
        private static Lazy<IConfiguration> _configuration = new Lazy<IConfiguration>(BuildConfiguration);
        private static Lazy<ServiceProvider> _serviceProvider = new Lazy<ServiceProvider>(BuildServices);

        private static IConfiguration Configuration
        {
            get { return _configuration.Value; }
        }

        private static ServiceProvider ServiceProvider
        {
            get { return _serviceProvider.Value; }
        }

        private static ExecutionContext _context;

        [FunctionName("OfficeAFunction")]
        public static async Task Run([ServiceBusTrigger("officea", "officeaprocessor", Connection = "OfficeAProcessorSubscriptionConnection", IsSessionsEnabled = true)]
        string message, ILogger log, ExecutionContext context)
        {
            _context = context;
            log.LogInformation($"C# ServiceBus topic trigger function processed message: {message}");

            var msg = JsonConvert.DeserializeObject<OfficeAMessage>(message);
            await ServiceProvider.GetService<OfficeAProcessingService>().Process(msg);
        }

        private static IConfiguration BuildConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(_context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
        }

        private static ServiceProvider BuildServices()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection
                .AddOptions()
                .Configure<OfficeAProcessingServiceOptions>(options => Configuration.Bind("OfficeAProcessingService", options))
                .Configure<PushedMessageRepositoryOptions>(options => Configuration.Bind("PushedMessageRepository", options))
                .AddScoped<OfficeAProcessingService>()
                .AddScoped<PushedMessageRepository>();

            return serviceCollection.BuildServiceProvider();
        }
    }
}
