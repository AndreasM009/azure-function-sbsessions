using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;

namespace OfficeAFunctionApp
{
    public class OfficeAProcessingService
    {
        private readonly OfficeAProcessingServiceOptions _options;
        private readonly PushedMessageRepository _repository;

        public OfficeAProcessingService(
            IOptions<OfficeAProcessingServiceOptions> options,
            PushedMessageRepository repository)
        {
            _options = options.Value;
            _repository = repository;
        }

        private TopicClient CreateTopicClient()
        {
            return new TopicClient(_options.ReadyForPickupTopicConnectionstring, _options.ReadyForPickupTopicName);
        }

        public async Task Process(OfficeAMessage message)
        {
            var msg = new ReadyForPickupMessage
            {
                Id = message.Id,
                CustomerId = message.CustomerId,
                BlobName = message.BlobName,
                RegistrationOffice = message.RegistrationOffice
            };

            var client = CreateTopicClient();
            var json = JsonConvert.SerializeObject(msg);
            var sbMsg = new Message(Encoding.UTF8.GetBytes(json)) { PartitionKey = message.CustomerId.ToString() };
            await client.SendAsync(sbMsg);
        }
    }
}
