using DurableFunctionsDemo.Common.ActivityFunctions;
using DurableFunctionsDemo.Common.Models;
using DurableFunctionsDemo.Common.SignalR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;
using static DurableFunctionsDemo.Common.Enums;

namespace DurableFunctionsDemo.Patterns.CoffeeMaker;

public class CoffeeMakerOrchestrator
{
    private readonly ICoffeeShopHubClient _coffeeProcessStatusPublisher;

    public CoffeeMakerOrchestrator(ICoffeeShopHubClient coffeeProcessStatusPublisher)
    {
        _coffeeProcessStatusPublisher = coffeeProcessStatusPublisher;
    }

    [Function(nameof(CoffeeMakerOrchestrator))]
    public async Task<Coffee> RunOrchestrator(
        [OrchestrationTrigger] TaskOrchestrationContext context)
    {
        ILogger logger = context.CreateReplaySafeLogger(nameof(CoffeeMakerOrchestrator));
        var coffee = context.GetInput<Coffee>();

        try
        {
            if (coffee == null)
            {
                coffee = new Coffee
                {
                    Status = CoffeeStatus.Failed,
                    Message = "Coffee specification not found."
                };

                logger.LogInformation(coffee.Message);

                await context.CallActivityAsync("UpdateCoffeeMakerStatus", coffee);

                return coffee;
            }

            coffee.Message = $"Started making {coffee.Type} coffee {coffee.Id} for order {coffee.OrderId}.";
            await context.CallActivityAsync("UpdateCoffeeMakerStatus", coffee);

            coffee = await context.CallActivityAsync<Coffee>(nameof(Grind), coffee);
            coffee = await context.CallActivityAsync<Coffee>(nameof(Dose), coffee);
            coffee = await context.CallActivityAsync<Coffee>(nameof(Tamp), coffee);
            coffee = await context.CallActivityAsync<Coffee>(nameof(Brew), coffee);
            
            if (coffee.Status == CoffeeStatus.Brewed)
            {
                coffee.Message = $"{coffee.Type} coffee {coffee.Id} for order {coffee.OrderId} is ready.";
                await context.CallActivityAsync("UpdateCoffeeMakerStatus", coffee);
            }
        }
        catch (Exception ex)
        {
            coffee.Status = CoffeeStatus.Failed;
            coffee.Message = ($"Coffee maker failed to complete {coffee.Type} coffee {coffee.Id} for order {coffee.OrderId}. " +
                $"Last known status: {coffee.Status}. Exception: {ex.Message}");

            logger.LogInformation(coffee.Message);

            await context.CallActivityAsync("UpdateCoffeeMakerStatus", coffee);
        }

        logger.LogInformation(coffee.Message);

        return coffee;
    }
}
