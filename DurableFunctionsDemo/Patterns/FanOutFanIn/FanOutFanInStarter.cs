using DurableFunctionsDemo.Common.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DurableFunctionsDemo.Patterns.Chaining;

public static class FanOutFanInStarter
{
    [Function(nameof(StartFanOutFanInOrchestrator))]
    public static async Task<HttpResponseData> StartFanOutFanInOrchestrator(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req,
        [DurableClient] DurableTaskClient client,
        FunctionContext executionContext)
    {
        ILogger logger = executionContext.GetLogger("StartFanOutFanInOrchestrator");

        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var coffeeOrder = JsonConvert.DeserializeObject<CoffeeOrder>(requestBody);

        // Function input comes from the request content.
        string instanceId = await client.ScheduleNewOrchestrationInstanceAsync(
            nameof(FanOutFanInOrchestrator), coffeeOrder);

        logger.LogInformation("Started Coffee Order Orchestration with ID = '{instanceId}'.", instanceId);

        // Returns an HTTP 202 response with an instance management payload.
        // See https://learn.microsoft.com/azure/azure-functions/durable/durable-functions-http-api#start-orchestration
        return client.CreateCheckStatusResponse(req, instanceId);
    }
}

