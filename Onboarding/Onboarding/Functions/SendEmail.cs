using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Onboarding.Models;
using SendGrid.Helpers.Mail;

namespace Onboarding.Functions
{
    public static class SendEmail
    {
        [FunctionName(nameof(SendEmail))]
        public static void Run(
            [QueueTrigger("emailqueue", Connection = "AzureWebJobsStorage")] string myQueueItem,
            [SendGrid(ApiKey = "SendGridApiKey")] out SendGridMessage message,
            ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");

            var onboardingData = JsonConvert.DeserializeObject<OnboardingInputData>(myQueueItem);

            message = new SendGridMessage();

            var messageText = $"Say hello to {onboardingData.FirstName} {onboardingData.LastName} ({onboardingData.Position}) from {onboardingData.Office}.";

            message.AddTo(onboardingData.Email);
            message.AddContent("text/plain", messageText);
            message.SetFrom(new EmailAddress("no-reply@onboarding.com"));
            message.SetSubject("New employee on board!");
        }
    }
}
 