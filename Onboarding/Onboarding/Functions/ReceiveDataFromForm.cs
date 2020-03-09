using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using Onboarding.Models;
using System.IO;
using System.Threading.Tasks;

namespace Onboarding.Functions
{
    public static class ReceiveDataFromForm
    {
        [FunctionName(nameof(ReceiveDataFromForm))]
        public static async Task<IActionResult> Run(
              [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
              [Queue("emailQueue", Connection = "StorageConnectionString")] CloudQueue emailQueue,
              [Queue("tableQueue", Connection = "StorageConnectionString")] CloudQueue tableQueue,
              ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            OnboardingInputData inputData = JsonConvert.DeserializeObject<OnboardingInputData>(requestBody);

            if (!inputData.Validate())
                return new BadRequestResult();

            string rawInputData = JsonConvert.SerializeObject(inputData);

            await emailQueue.AddMessageAsync(new CloudQueueMessage(rawInputData));
            await tableQueue.AddMessageAsync(new CloudQueueMessage(rawInputData));

            return new AcceptedResult();
        }
    }
}
