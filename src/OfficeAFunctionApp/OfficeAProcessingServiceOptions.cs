using System;
using System.Collections.Generic;
using System.Text;

namespace OfficeAFunctionApp
{
    public class OfficeAProcessingServiceOptions
    {
        public string ReadyForPickupTopicConnectionstring { get; set; }
        public string ReadyForPickupTopicName { get; set; }
        public string StorageAccountConnectionString { get; set; }
        public string BlobContainerName { get; set; }
    }
}
