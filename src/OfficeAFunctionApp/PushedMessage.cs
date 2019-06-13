using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace OfficeAFunctionApp
{
    public class PushedMessage : TableEntity
    {
        public PushedMessage()
        {

        }

        public PushedMessage(string registrationOffice, Guid msgId)
        {
            PartitionKey = registrationOffice;
            RowKey = msgId.ToString();
        }

        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public string BlobName { get; set; }
        public string RegistrationOffice { get; set; }
    }
}
