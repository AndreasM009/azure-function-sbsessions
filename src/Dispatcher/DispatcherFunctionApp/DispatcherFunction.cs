using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DispatcherFunctionApp
{
    public static class DispatcherFunction
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

        [FunctionName("DispatcherFunction")]
        public static async Task Run([ServiceBusTrigger("submitted", "dispatcher", Connection = "DispatcherSbSubscriptionConnection", IsSessionsEnabled = true)]
            string message, 
            ILogger log,
            ExecutionContext context)
        {
            _context = context;

            var msg = JsonConvert.DeserializeObject<Message>(message);
            log.LogInformation($"C# ServiceBus topic trigger function processed message: {message}");
            await ServiceProvider.GetService<DispatcherService>().Dispatch(msg);
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
                .Configure<DispatchServiceOptions>(options => Configuration.Bind("DispatchService", options))
                .AddScoped<DispatcherService>();

            return serviceCollection.BuildServiceProvider();
        }
    }
}
