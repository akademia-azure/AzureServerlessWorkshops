using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OnboardingWithEventGrid.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace OnboardingWithEventGrid.Functions
{
    public class SubmitToEventGrid
    {
        [FunctionName("SubmitToEventGrid")]
        public static async Task<IActionResult> Run(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
           ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            OnboardingDataEntity inputData = JsonConvert.DeserializeObject<OnboardingDataEntity>(requestBody);
            inputData.PartitionKey = "onboardingData";
            inputData.RowKey = Guid.NewGuid().ToString();

            if (!inputData.Validate())
                return new BadRequestResult();

            var events = new List<Event>
            {
                new Event()
                {
                    id = Guid.NewGuid().ToString(),
                    eventTime = DateTime.UtcNow,
                    eventType = "onboarding",
                    subject = "new",
                    data = JsonConvert.SerializeObject(inputData)
                }
            };

            var httpClient = new HttpClient();
            var url = Environment.GetEnvironmentVariable("EventGridUrl");
            httpClient.DefaultRequestHeaders.Add("aeg-sas-key", Environment.GetEnvironmentVariable("EventGridSasKey"));
            await httpClient.PostAsJsonAsync(Environment.GetEnvironmentVariable("EventGridUrl"), events);

            return new AcceptedResult();
        }
    }
}
