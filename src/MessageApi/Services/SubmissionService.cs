using MessageApi.Messages;
using MessageApi.Repositories;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace MessageApi.Services
{
    public class SubmissionService
    {
        private readonly DutyMessageRepository _repository;
        private readonly SubmissionServiceOptions _options;
        private static bool _blobContainerCreated = false;
        private static object _syncRoot = new object();

        public SubmissionService(IOptions<SubmissionServiceOptions> options, DutyMessageRepository repository)
        {
            _repository = repository;
            _options = options.Value;
        }

        private TopicClient CreateTopicClient()
        {
            return new TopicClient(_options.ServiceBusConnectionString, _options.TopicName);
        }

        private CloudBlobContainer CreateBlobContainer()
        {
            var account = CloudStorageAccount.Parse(_options.StorageAccountConnectionString);
            var blobClient = account.CreateCloudBlobClient();
            var blobContainer = blobClient.GetContainerReference(_options.BlobContainerName);

            if (!_blobContainerCreated)
            {
                lock (_syncRoot)
                {
                    if (!_blobContainerCreated)
                    {
                        blobContainer.CreateIfNotExists();
                        _blobContainerCreated = true;
                    }
                }
            }

            return blobContainer;
        }

        public async Task Submit(Guid customerId, IEnumerable<Guid> messageIds)
        {
            var messages = await _repository.GetByCustomerIdAndMessageIds(customerId, messageIds);
            var client = CreateTopicClient();
            
            using (var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                foreach (var msg in messages)
                {
                    var json = JsonConvert.SerializeObject(msg);
                    var blobReferenceName = $"{customerId}-{msg.Id}.json";
                    var container = CreateBlobContainer();
                    var blob = container.GetBlockBlobReference(blobReferenceName);

                    blob.Properties.ContentType = "application/json";

                    await blob.UploadTextAsync(json);

                    var message = new SubmitMessage
                    {
                        Id = msg.Id,
                        CustomerId = customerId,
                        BlobName = blobReferenceName,
                        RegistrationOffice = msg.RegistrationOffice
                    };

                    var messageJson = JsonConvert.SerializeObject(message);

                    await client.SendAsync(new Message(Encoding.UTF8.GetBytes(messageJson))
                    {
                        SessionId = customerId.ToString()
                    });
                }

                ts.Complete();
            }
        }
    }
}
