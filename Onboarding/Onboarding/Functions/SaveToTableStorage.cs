using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using Onboarding.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Onboarding.Functions
{
    public static class SaveToTableStorage
    {
        [FunctionName("SaveToTableStorage")]
        public static async Task Run(
            [QueueTrigger("tablequeue", Connection = "StorageConnectionString")] string myQueueItem,
            [Table("onboardingtable", Connection = "StorageConnectionString")] CloudTable onboardingTable,
            ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");

            OnboardingDataEntity onboardingData = JsonConvert.DeserializeObject<OnboardingDataEntity>(myQueueItem);
            onboardingData.PartitionKey = "onboardingData";
            onboardingData.RowKey = Guid.NewGuid().ToString();

            TableOperation tableOperation = TableOperation.InsertOrMerge(onboardingData);

            TableResult result = await onboardingTable.ExecuteAsync(tableOperation);
        }
    }
}
