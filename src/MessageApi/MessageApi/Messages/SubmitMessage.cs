using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessageApi.Messages
{
    public class SubmitMessage
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public string BlobName { get; set; }
        public string RegistrationOffice { get; set; }
    }
}
