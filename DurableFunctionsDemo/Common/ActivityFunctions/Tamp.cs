using DurableFunctionsDemo.Common.Models;
using DurableFunctionsDemo.Common.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using static DurableFunctionsDemo.Common.Enums;

namespace DurableFunctionsDemo.Common.ActivityFunctions;
public class Tamp
{
    private readonly ICoffeeShopHubClient _coffeeProcessStatusPublisher;

    public Tamp(ICoffeeShopHubClient coffeeProcessStatusPublisher)
    {
        _coffeeProcessStatusPublisher = coffeeProcessStatusPublisher;
    }

    [Function(nameof(Tamp))]
    public async Task<Coffee> Run([ActivityTrigger] Coffee coffee, FunctionContext executionContext)
    {
        ILogger logger = executionContext.GetLogger(nameof(Tamp));

        logger.LogInformation($"Tamping {coffee.Type} coffee {coffee.Id} for order: {coffee.OrderId} ...");

        await Task.Delay(2000);

        coffee.Status = CoffeeStatus.Tamped;
        coffee.Message = "Tamping Completed";

        logger.LogInformation($"Tamped {coffee.Type} coffee {coffee.Id} of order: {coffee.OrderId}.");

        await _coffeeProcessStatusPublisher.UpdateCoffeeProcessStatus(coffee);

        return coffee;
    }
}