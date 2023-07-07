using DurableFunctionsDemo.Common.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DurableFunctionsDemo.Patterns.HumanInteraction;

public static class HumanInteractionStarter
{
    [Function(nameof(StartHumanInteraction))]
    public static async Task<HttpResponseData> StartHumanInteraction(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req,
        [DurableClient] DurableTaskClient client,
        FunctionContext executionContext)
    {
        ILogger logger = executionContext.GetLogger("StartHumanInteraction");

        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var coffeeOrder = JsonConvert.DeserializeObject<CoffeeOrder>(requestBody);

        // Function input comes from the request content.
        string instanceId = await client.ScheduleNewOrchestrationInstanceAsync(
            nameof(HumanInteractionOrchestrator), coffeeOrder);

        logger.LogInformation("Started Coffee Shop Orchestration with ID = '{instanceId}'.", instanceId);

        // Returns an HTTP 202 response with an instance management payload.
        // See https://learn.microsoft.com/azure/azure-functions/durable/durable-functions-http-api#start-orchestration
        return client.CreateCheckStatusResponse(req, instanceId);
    }
}

