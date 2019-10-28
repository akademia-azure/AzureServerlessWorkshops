using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace Logger
{
    public static class Function1
    {
        [FunctionName("Logger")]
        public static void Run([TimerTrigger("*/30 * * * * *")]TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            var format = Environment.GetEnvironmentVariable("Format");

            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now.ToString(format)}");
        }
    }
}
