using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Text;

namespace MessageSenderClient
{
    class Program
    {
        private const string ConnectionString = "Endpoint=sb://anmockfunc.servicebus.windows.net/;SharedAccessKeyName=submittedSend;SharedAccessKey=ts4wqtqVqxqapOc7RYZ36mLHnFHfzu1kZ6kIWs/BLP0=;";
        private const string TopicName = "submitted";

        static void Main(string[] args)
        {
            var client = new TopicClient(ConnectionString, TopicName);

            var msg = new Message
            {
                CustomerId = Guid.NewGuid(),
                Id = Guid.NewGuid(),
                BlobUri = "messages/testblob",
                RegistrationOffice = "OfficeA"
            };

            var json = JsonConvert.SerializeObject(msg);
            var sbMsg = new Microsoft.Azure.ServiceBus.Message(Encoding.UTF8.GetBytes(json));
            sbMsg.SessionId = msg.CustomerId.ToString();
            client.SendAsync(sbMsg).GetAwaiter().GetResult();
        }
    }
}
