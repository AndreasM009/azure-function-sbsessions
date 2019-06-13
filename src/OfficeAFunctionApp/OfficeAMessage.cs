using System;
using System.Collections.Generic;
using System.Text;

namespace OfficeAFunctionApp
{
    public class OfficeAMessage
    {
        public Guid CustomerId { get; set; }
        public Guid Id { get; set; }
        public string BlobName { get; set; }
        public string RegistrationOffice { get; set; }
    }
}
