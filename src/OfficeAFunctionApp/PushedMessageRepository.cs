using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OfficeAFunctionApp
{
    public class PushedMessageRepository
    {
        private readonly PushedMessageRepositoryOptions _options;
        private static bool _tableCreated = false;
        private static object _syncRoot = new object();

        public PushedMessageRepository(IOptions<PushedMessageRepositoryOptions> options)
        {
            _options = options.Value;
        }

        private CloudTable CreateTable()
        {
            var storageAccount = CloudStorageAccount.Parse(_options.ConnectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference(_options.TableName);

            if (!_tableCreated)
            {
                lock (_syncRoot)
                {
                    if (!_tableCreated)
                    {
                        table.CreateIfNotExists();
                        _tableCreated = true;
                    }
                }                
            }

            return table;
        }

        public async Task<List<PushedMessage>> Get(string registrationOffice)
        {
            var table = CreateTable();

            var query = new TableQuery<PushedMessage>()
                .Where(TableQuery.GenerateFilterCondition(
                    nameof(TableEntity.PartitionKey),
                    QueryComparisons.Equal,
                    registrationOffice));

            var result = new List<PushedMessage>();
            TableContinuationToken token = null;

            do
            {
                var queryResult = await table.ExecuteQuerySegmentedAsync(query, token);
                result.AddRange(queryResult);
                token = queryResult.ContinuationToken;
            } while (token != null);

            return result;
        }

        public async Task<List<PushedMessage>> DeleteAll(string registrationOffice, List<PushedMessage> messages)
        {
            var table = CreateTable();
            
            var batch = new TableBatchOperation();

            foreach (var m in messages)
                batch.Delete(m);

            await table.ExecuteBatchAsync(batch);

            return messages;
        }

        public async Task<PushedMessage> Add(PushedMessage message)
        {
            var table = CreateTable();
            var op = TableOperation.Insert(message);
            await table.ExecuteAsync(op);
            return message;
        }
    }
}
