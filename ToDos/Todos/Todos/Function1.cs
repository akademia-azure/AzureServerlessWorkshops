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

namespace Todos
{
    public static class Function1
    {
        private static List<Todo> Todos = new List<Todo>()
        {
            new Todo
            {
                Title = "Do something",
                Checked = false
            },
            new Todo
            {
                Title = "Do something 2",
                Checked = true
            }
        };

        [FunctionName("GetAll")]
        public static async Task<IEnumerable<Todo>> GetAll(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "all")] HttpRequest req)
        {
            return Todos;
        }

        [FunctionName("Delete")]
        public static async Task<IActionResult> Delete(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "delete")] HttpRequest req)
        {
            string id = req.Query["id"];

            if (!string.IsNullOrEmpty(id))
            {
                var itemToRemove = Todos.Where(t => t.Id == id).FirstOrDefault();
                Todos.Remove(itemToRemove);

                return new OkObjectResult($"Todo was deleted");
            }

            return new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }

        [FunctionName("Toggle")]
        public static async Task<IActionResult> Toggle(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "toggle")] HttpRequest req)
        {
            string id = req.Query["id"];

            if (!string.IsNullOrEmpty(id))
            {
                var itemToToggle = Todos.Where(t => t.Id == id).FirstOrDefault();
                itemToToggle.Checked = !itemToToggle.Checked;

                return new OkObjectResult($"Todo was toggled");
            }

            return new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }

        [FunctionName("Add")]
        public static async Task<IActionResult> Add(
           [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "add")] HttpRequest req)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            if(data?.title == null)
            {
                return new BadRequestObjectResult("Please pass a name on the query string or in the request body");
            }

            Todos.Add(new Todo
            {
                Id = Todos.Max(t => t.Id) + 1,
                Title = data.title,
                Checked = false
            });

            return new OkObjectResult($"Todo was added");
        }
    }
}
