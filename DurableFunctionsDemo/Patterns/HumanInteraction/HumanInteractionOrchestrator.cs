using DurableFunctionsDemo.Common.Models;
using DurableFunctionsDemo.Patterns.CoffeeMaker;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;
using static DurableFunctionsDemo.Common.Enums;

namespace DurableFunctionsDemo.Patterns.HumanInteraction;

public class HumanInteractionOrchestrator
{
    [Function(nameof(HumanInteractionOrchestrator))]
    public static async Task<CoffeeOrder> RunOrchestrator(
        [OrchestrationTrigger] TaskOrchestrationContext context)
    {
        ILogger logger = context.CreateReplaySafeLogger(nameof(HumanInteractionOrchestrator));
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

            coffeeOrder = await context.CallActivityAsync<CoffeeOrder>("CalculateOrderPrice", coffeeOrder);

            await context.CallActivityAsync("SendPaymentRequest", coffeeOrder);

            coffeeOrder.Status = OrderStatus.Placed;
            coffeeOrder.Message = $"Coffee order {coffeeOrder.Id} was placed. Total Cost: {coffeeOrder.TotalCost:C}. Waiting for payment confirmation...";
            await context.CallActivityAsync("UpdateCoffeeOrderStatus", coffeeOrder);

            var response = await context.WaitForExternalEvent<ProcessPaymentEventModel>("ProcessPayment");

            if (response.PaymentStatus == PaymentStatus.Approved)
            {
                coffeeOrder.Status = OrderStatus.Placed;
                coffeeOrder.Message = $"Payment received for coffee order {coffeeOrder.Id}";
                await context.CallActivityAsync("UpdateCoffeeOrderStatus", coffeeOrder);

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
            else
            {
                coffeeOrder.Status = OrderStatus.Completed;
                coffeeOrder.Message = $"Failed to receive payment for {coffeeOrder.Id}.";
                await context.CallActivityAsync("UpdateCoffeeOrderStatus", coffeeOrder);
            }
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


