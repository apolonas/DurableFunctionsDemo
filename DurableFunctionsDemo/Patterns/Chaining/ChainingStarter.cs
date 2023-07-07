using DurableFunctionsDemo.Common.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DurableFunctionsDemo.Patterns.Chaining;

public static class ChainingStarter
{
    [Function(nameof(StartChainingOrchestrator))]
    public static async Task<HttpResponseData> StartChainingOrchestrator(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req,
        [DurableClient] DurableTaskClient client,
        FunctionContext executionContext)
    {
        ILogger logger = executionContext.GetLogger("StartChainingOrchestrator");

        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var coffeeOrder = JsonConvert.DeserializeObject<CoffeeOrder>(requestBody);

        // Function input comes from the request content.
        string instanceId = await client.ScheduleNewOrchestrationInstanceAsync(
            nameof(ChainingOrchestrator), coffeeOrder);

        logger.LogInformation("Started Coffee Order Orchestration with ID = '{instanceId}'.", instanceId);

        // Returns an HTTP 202 response with an instance management payload.
        // See https://learn.microsoft.com/azure/azure-functions/durable/durable-functions-http-api#start-orchestration
        return client.CreateCheckStatusResponse(req, instanceId);
    }
}

