using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessageApi.Services
{
    public class SubmissionServiceOptions
    {
        public string ServiceBusConnectionString { get; set; }
        public string TopicName { get; set; }
        public string StorageAccountConnectionString { get; set; }
        public string BlobContainerName { get; set; }
    }
}
