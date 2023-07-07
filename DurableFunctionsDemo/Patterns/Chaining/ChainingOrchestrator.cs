using DurableFunctionsDemo.Common.ActivityFunctions;
using DurableFunctionsDemo.Common.Models;
using DurableFunctionsDemo.Common.SignalR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;
using static DurableFunctionsDemo.Common.Enums;

namespace DurableFunctionsDemo.Patterns.Chaining;

public class ChainingOrchestrator
{
    private readonly ICoffeeShopHubClient _coffeeProcessStatusPublisher;

    public ChainingOrchestrator(ICoffeeShopHubClient coffeeProcessStatusPublisher)
    {
        _coffeeProcessStatusPublisher = coffeeProcessStatusPublisher;
    }

    [Function(nameof(ChainingOrchestrator))]
    public static async Task<CoffeeOrder> RunOrchestrator(
        [OrchestrationTrigger] TaskOrchestrationContext context)
    {
        ILogger logger = context.CreateReplaySafeLogger(nameof(ChainingOrchestrator));
        var coffeeOrder = context.GetInput<CoffeeOrder>();

        try
        {
            if (coffeeOrder == null)
            {
                coffeeOrder = new CoffeeOrder
                {
                    Status = OrderStatus.Failed,
                    Message = "Coffee order not specified."
                };

                logger.LogInformation(coffeeOrder.Message);

                await context.CallActivityAsync("UpdateCoffeeOrderStatus", coffeeOrder);

                return coffeeOrder;
            }

            coffeeOrder.Status = OrderStatus.Started;
            coffeeOrder.Message = $"Started processing coffee order {coffeeOrder.Id}.";
            await context.CallActivityAsync("UpdateCoffeeOrderStatus", coffeeOrder);

            foreach (var coffee in coffeeOrder.Coffees)
            {
                coffee.Message = $"Started making {coffee.Type} coffee {coffee.Id} for order {coffee.OrderId}.";
                await context.CallActivityAsync("UpdateCoffeeMakerStatus", coffee);

                var processedCoffee = await context.CallActivityAsync<Coffee>(nameof(Grind), coffee);
                processedCoffee = await context.CallActivityAsync<Coffee>(nameof(Dose), processedCoffee);
                processedCoffee = await context.CallActivityAsync<Coffee>(nameof(Tamp), processedCoffee);
                processedCoffee = await context.CallActivityAsync<Coffee>(nameof(Brew), processedCoffee);

                if (processedCoffee.Status == CoffeeStatus.Brewed)
                {
                    processedCoffee.Message = $"{processedCoffee.Type} coffee {processedCoffee.Id} " +
                        $"for order {processedCoffee.OrderId} is ready.";

                    await context.CallActivityAsync("UpdateCoffeeMakerStatus", processedCoffee);
                }
            }

            coffeeOrder.Status = OrderStatus.Completed;
            coffeeOrder.Message = $"Coffee order {coffeeOrder.Id} is ready.";
            await context.CallActivityAsync("UpdateCoffeeOrderStatus", coffeeOrder);
        }
        catch (Exception ex)
        {
            coffeeOrder.Status = OrderStatus.Failed;
            coffeeOrder.Message = $"Failed to complete order {coffeeOrder.Id}. Exception: {ex.Message}";

            logger.LogInformation(coffeeOrder.Message);

            await context.CallActivityAsync("UpdateCoffeeOrderStatus", coffeeOrder);
        }

        logger.LogInformation(coffeeOrder.Message);

        return coffeeOrder;
    }
}

