using DurableFunctionsDemo.Common.Models;
using DurableFunctionsDemo.Common.SignalR;
using DurableFunctionsDemo.Patterns.CoffeeMaker;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;
using static DurableFunctionsDemo.Common.Enums;

namespace DurableFunctionsDemo.Patterns.Chaining;

public class FanOutFanInOrchestrator
{
    private readonly ICoffeeShopHubClient _coffeeProcessStatusPublisher;

    public FanOutFanInOrchestrator(ICoffeeShopHubClient coffeeProcessStatusPublisher)
    {
        _coffeeProcessStatusPublisher = coffeeProcessStatusPublisher;
    }

    [Function(nameof(FanOutFanInOrchestrator))]
    public static async Task<CoffeeOrder> RunOrchestrator(
        [OrchestrationTrigger] TaskOrchestrationContext context)
    {
        ILogger logger = context.CreateReplaySafeLogger(nameof(FanOutFanInOrchestrator));
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

            var coffeeTasks = new List<Task<Coffee>>();

            coffeeOrder.Status = OrderStatus.Started;
            coffeeOrder.Message = $"Started processing coffee order {coffeeOrder.Id}.";
            await context.CallActivityAsync("UpdateCoffeeOrderStatus", coffeeOrder);

            foreach (var coffee in coffeeOrder.Coffees)
            {
                var task = context.CallSubOrchestratorAsync<Coffee>(nameof(CoffeeMakerOrchestrator), coffee);
                coffeeTasks.Add(task);
            }

            var coffeeMessages = await Task.WhenAll(coffeeTasks);

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

