using System;
using System.Collections.Generic;
using System.Text;

namespace OfficeAFunctionApp
{
    public class ReadyForPickupMessage
    {
        public Guid CustomerId { get; set; }
        public Guid Id { get; set; }
        public string BlobName { get; set; }
        public string RegistrationOffice { get; set; }
    }
}
