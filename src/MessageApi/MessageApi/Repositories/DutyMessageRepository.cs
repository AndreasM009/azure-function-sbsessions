using MessageApi.DomainObjects;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessageApi.Repositories
{
    public class DutyMessageRepository
    {
        private readonly DutyMessageRepositoryOptions _options;
        private readonly Lazy<CloudTable> _table;
        private static bool _tableCreated = false;

        public DutyMessageRepository(IOptions<DutyMessageRepositoryOptions> options)
        {
            _options = options.Value;
            _table = new Lazy<CloudTable>(() => CreateTable());
        }

        private CloudTable CreateTable()
        {
            var storageAccount = CloudStorageAccount.Parse(_options.ConnectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference(_options.TableName);

            if (!_tableCreated)
                table.CreateIfNotExists();

            _tableCreated = true;

            return table;
        }

        private CloudTable Table
        {
            get { return _table.Value; }
        }

        public async Task<List<DutyMessage>> Add(Guid customerId, List<DutyMessage> messages)
        {
            var op = new TableBatchOperation();
            foreach (var m in messages)
                op.Insert(m);

            await Table.ExecuteBatchAsync(op);
            return messages;
        }

        public async Task<List<DutyMessage>> GetByCustomerId(Guid CustomerId)
        {
            var table = Table;

            var query = new TableQuery<DutyMessage>()
                .Where(TableQuery.GenerateFilterCondition(
                    nameof(TableEntity.PartitionKey), 
                    QueryComparisons.Equal, 
                    CustomerId.ToString()));

            var result = new List<DutyMessage>();
            TableContinuationToken token = null;

            do
            {
                var queryResult = await table.ExecuteQuerySegmentedAsync(query, token);
                result.AddRange(queryResult);
                token = queryResult.ContinuationToken;
            } while (token != null);

            return result;
        }

        public async Task<List<DutyMessage>> GetByCustomerIdAndMessageIds(Guid customerId, IEnumerable<Guid> messageIds)
        {
            var table = Table;
            var result = new List<DutyMessage>();

            foreach (var id in messageIds)
            {
                var op = TableOperation.Retrieve<DutyMessage>(customerId.ToString(), id.ToString());
                var msg = (DutyMessage)(await table.ExecuteAsync(op)).Result;
                result.Add(msg);
            }

            return result;
        }
    }
}
