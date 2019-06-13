using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DispatcherFunctionApp
{
    public class DispatcherService
    {
        private readonly DispatchServiceOptions _options;
        
        public DispatcherService(IOptions<DispatchServiceOptions> options)
        {
            _options = options.Value;
        }

        private Dictionary<string, TopicClient> CreateTopicClients()
        {
            var d = new Dictionary<string, TopicClient>
            {
                { "OfficeA", new TopicClient(_options.RegistrationOfficeATopicConnectionString, _options.RegistrationOfficeATopicName) },
                { "OfficeB", new TopicClient(_options.RegistrationOfficeBTopicConnectionString, _options.RegistrationOfficeBTopicName) },
                { "OfficeC", new TopicClient(_options.RegistrationOfficeCTopicConnectionString, _options.RegistrationOfficeCTopicName) }
            };

            return d;
        }

        public async Task Dispatch(Message msg)
        {
            var clients = CreateTopicClients();
            
            var officeMsg = new RegistrationOfficeMessage
            {
                Id = msg.Id,
                CustomerId = msg.CustomerId,
                RegistrationOffice = msg.RegistrationOffice,
                BlobName = msg.BlobName
            };

            var officeMsgJson = JsonConvert.SerializeObject(officeMsg);

            await clients[msg.RegistrationOffice]
                .SendAsync(new Microsoft.Azure.ServiceBus.Message(Encoding.UTF8.GetBytes(officeMsgJson))
                {
                    SessionId = msg.RegistrationOffice
                });
        }
    }
}
