// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}

using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using UxMaterialsEventGrid.Models;
using System.Threading.Tasks;

namespace UxMaterialsEventGrid
{
    public static class HandleUxMaterialsEvent
    {
        [FunctionName(nameof(HandleUxMaterialsEvent))]
        public static async Task Run([EventGridTrigger]EventGridEvent eventGridEvent, 
            [Table("uxMaterials", Connection = "StorageConnectionString")] IAsyncCollector<MaterialsMetadata> todoTable, 
            ILogger log)
        {
            dynamic eventData = eventGridEvent.Data;

            log.LogInformation(eventGridEvent.Data.ToString());

            var todo = new MaterialsMetadata
            {
                Path = eventData.path,
                ContentType = eventData.contentType,
                ContentLength = eventData.contentLength
            };

            todo.PartitionKey = "uxMaterials";
            todo.RowKey = Guid.NewGuid().ToString();

            await todoTable.AddAsync(todo);
        }
    }
}
