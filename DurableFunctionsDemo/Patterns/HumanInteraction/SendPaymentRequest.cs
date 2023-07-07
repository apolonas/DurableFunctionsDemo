using DurableFunctionsDemo.Common.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace DurableFunctionsDemo.Patterns.HumanInteraction;

public static class SendPaymentRequest
{
    [Function(nameof(SendPaymentRequest))]
    public static async Task Run([ActivityTrigger] CoffeeOrder coffeeOrder, FunctionContext executionContext)
    {
        ILogger logger = executionContext.GetLogger(nameof(SendPaymentRequest));

        logger.LogInformation($"Sending payment request for order: {coffeeOrder.Id} ...");

        await Task.Delay(2000);
    }
}