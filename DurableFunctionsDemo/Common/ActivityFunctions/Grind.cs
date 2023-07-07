using DurableFunctionsDemo.Common.Models;
using DurableFunctionsDemo.Common.SignalR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using static DurableFunctionsDemo.Common.Enums;

namespace DurableFunctionsDemo.Common.ActivityFunctions;

public class Grind
{
    private readonly ICoffeeShopHubClient _coffeeProcessStatusPublisher;

    public Grind(ICoffeeShopHubClient coffeeProcessStatusPublisher)
    {
        _coffeeProcessStatusPublisher = coffeeProcessStatusPublisher;
    }

    [Function(nameof(Grind))]
    public async Task<Coffee> Run([ActivityTrigger] Coffee coffee, FunctionContext executionContext)
    {
        ILogger logger = executionContext.GetLogger(nameof(Grind));

        logger.LogInformation($"Grinding {coffee.Type} coffee  {coffee.Id} for order: {coffee.OrderId} ...");

        await Task.Delay((int)coffee.Intensity * 1000);

        coffee.Status = CoffeeStatus.Ground;
        coffee.Message = "Grinding Completed";

        logger.LogInformation($"Coffee grinding completed for {coffee.Type} coffee  {coffee.Id} of order: {coffee.OrderId}.");

        await _coffeeProcessStatusPublisher.UpdateCoffeeProcessStatus(coffee);

        return coffee;
    }
}