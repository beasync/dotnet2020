using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace FunctionActors.Function
{
    public static class DurableFunctionsOrchestration
    {
        [FunctionName("FanOutFanIn")]
        public static async Task<string> FanOutFanIn(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var parallelTasks = new List<Task<int>>();
            var workBatch = await context.CallActivityAsync<int[][]>("WorkBatch", null);

            for (int i = 0; i < workBatch.Length; i++)
            {
                var task = context.CallActivityAsync<int>("Sum", workBatch[i]);
                parallelTasks.Add(task);
            }

            await Task.WhenAll(parallelTasks);

            var result = parallelTasks.Select(t => t.Result).ToArray();

            await context.CallActivityAsync("Out", result);

            return StatusCodes.Status200OK.ToString();
        }

        [FunctionName("WorkBatch")]
        public static int[][] WorkBatch([ActivityTrigger] IDurableActivityContext context)
        {
            return new int[][]
            {
                new int[] { 2, 4, 6, 8 },
                new int[] { 1, 3, 5, 7 }
            };
        }

        [FunctionName("Sum")]
        public static int Sum([ActivityTrigger] int[] input, ILogger log)
        {
            return input.Sum();
        }

        [FunctionName("Out")]
        public static void Out(
            [ActivityTrigger] int[] result,
            [Queue("outqueue"),StorageAccount("AzureWebJobsStorage")] ICollector<string> msg)
        {
            msg.Add(JsonSerializer.Serialize(result));
        }

        [FunctionName("Trigger")]
        public static async Task<HttpResponseMessage> Trigger(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            string instanceId = await starter.StartNewAsync("FanOutFanIn", null);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}