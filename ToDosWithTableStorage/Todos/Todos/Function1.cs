using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure.Storage.Table;

namespace Todos
{
    public static class Function1
    {
        [FunctionName("GetAll")]
        public static async Task<IEnumerable<TodoEntity>> GetAll(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "all")] HttpRequest req,
            [Table("ToDo", Connection = "AzureWebJobsStorage")] CloudTable todoTable)
        {
            var query = new TableQuery<TodoEntity>();
            var todos = await todoTable.ExecuteQuerySegmentedAsync(query, null);
            return todos.Select(t=>t);
        }

        [FunctionName("Delete")]
        public static async Task<IActionResult> Delete(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "delete")] HttpRequest req,
            [Table("ToDo", Connection = "AzureWebJobsStorage")] CloudTable todoTable)
        {
            string id = req.Query["id"];

            if (!string.IsNullOrEmpty(id))
            {
                var query = TableOperation.Retrieve<TodoEntity>("todo", id);
                var result = await todoTable.ExecuteAsync(query);

                if (result.Result == null)
                    return new NotFoundResult();

                var deleteOperation = TableOperation.Delete((TodoEntity)result.Result);
                await todoTable.ExecuteAsync(deleteOperation);

                return new OkObjectResult($"Todo was deleted");
            }

            return new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }

        [FunctionName("Toggle")]
        public static async Task<IActionResult> Toggle(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "toggle")] HttpRequest req,
            [Table("ToDo", Connection = "AzureWebJobsStorage")] CloudTable todoTable)
        {
            string id = req.Query["id"];

            if (!string.IsNullOrEmpty(id))
            {
                var query = TableOperation.Retrieve<TodoEntity>("todo", id);
                var result = await todoTable.ExecuteAsync(query);

                if (result.Result == null)
                    return new NotFoundResult();

                var existingRow = (TodoEntity)result.Result;
                existingRow.Checked = !existingRow.Checked;

                var replaceOperation = TableOperation.Replace(existingRow);
                await todoTable.ExecuteAsync(replaceOperation);

                return new OkObjectResult($"Todo was deleted");
            }

            return new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }

        [FunctionName("Add")]
        public static async Task<IActionResult> Add(
           [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "add")] HttpRequest req,
           [Table("ToDo", Connection = "AzureWebJobsStorage")] IAsyncCollector<TodoEntity> todoTable)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            if(data?.title == null)
            {
                return new BadRequestObjectResult("Please pass a name on the query string or in the request body");
            }

            var todo = new TodoEntity
            {
                Title = data.title,
                Checked = false,
            };

            todo.PartitionKey = "todo";
            todo.RowKey = todo.Id;

            await todoTable.AddAsync(todo);

            return new OkObjectResult($"Todo was added");
        }
    }
}
