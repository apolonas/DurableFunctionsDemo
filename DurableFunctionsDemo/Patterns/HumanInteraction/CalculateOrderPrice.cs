using DurableFunctionsDemo.Common;
using DurableFunctionsDemo.Common.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace DurableFunctionsDemo.Patterns.HumanInteraction;

public static class CalculateOrderPrice
{
    [Function(nameof(CalculateOrderPrice))]
    public static async Task<CoffeeOrder> Run([ActivityTrigger] CoffeeOrder coffeeOrder, FunctionContext executionContext)
    {
        ILogger logger = executionContext.GetLogger(nameof(CalculateOrderPrice));

        logger.LogInformation($"Calculating cost for order: {coffeeOrder.Id} ...");

        await Task.Delay(2000);

        var price = CoffeePriceCalculator.CalculateOrderPrice(coffeeOrder);

        coffeeOrder.TotalCost = price;

        logger.LogInformation($"Calculated cost for order: {coffeeOrder.Id}.");

        return coffeeOrder;
    }
}
