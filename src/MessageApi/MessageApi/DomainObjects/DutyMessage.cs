using Microsoft.Azure.Cosmos.Table;
using System;

namespace MessageApi.DomainObjects
{
    public class DutyMessage : TableEntity
    {
        public DutyMessage()
        {

        }

        public DutyMessage(Guid customerId, Guid id)
        {
            PartitionKey = customerId.ToString();
            RowKey = id.ToString();
            CustomerId = customerId;
            Id = id;
        }

        public Guid CustomerId { get; set; }
        public Guid Id { get; set; }
        public string RegistrationOffice { get; set; }
        public string Text { get; set; }
    }
}
