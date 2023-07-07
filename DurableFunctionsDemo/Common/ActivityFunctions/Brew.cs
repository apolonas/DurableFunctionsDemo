using DurableFunctionsDemo.Common.Models;
using DurableFunctionsDemo.Common.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Extensions.Logging;
using static DurableFunctionsDemo.Common.Enums;

namespace DurableFunctionsDemo.Common.ActivityFunctions;

public class Brew
{
    private readonly ICoffeeShopHubClient _coffeeProcessStatusPublisher;

    public Brew(ICoffeeShopHubClient coffeeProcessStatusPublisher)
    {
        _coffeeProcessStatusPublisher = coffeeProcessStatusPublisher;
    }

    [Function(nameof(Brew))]
    public async Task<Coffee> Run([ActivityTrigger] Coffee coffee, FunctionContext executionContext)
    {
        ILogger logger = executionContext.GetLogger(nameof(Brew));

        logger.LogInformation($"Brewing {coffee.Type} coffee  {coffee.Id} for order: {coffee.OrderId} ...");

        await Task.Delay((int)coffee.Intensity * 1000);

        coffee.Status = CoffeeStatus.Brewed;
        coffee.Message = "Brewing Completed";

        logger.LogInformation($"Brewing completed for {coffee.Type} coffee  {coffee.Id} of order: {coffee.OrderId}.");

        await _coffeeProcessStatusPublisher.UpdateCoffeeProcessStatus(coffee);

        return coffee;
    }
}
