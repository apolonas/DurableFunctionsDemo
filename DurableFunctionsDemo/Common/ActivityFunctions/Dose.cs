using DurableFunctionsDemo.Common.Models;
using DurableFunctionsDemo.Common.SignalR;
using Google.Protobuf;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using static DurableFunctionsDemo.Common.Enums;

namespace DurableFunctionsDemo.Common.ActivityFunctions;

public class Dose
{
    private readonly ICoffeeShopHubClient _coffeeProcessStatusPublisher;

    public Dose(ICoffeeShopHubClient coffeeProcessStatusPublisher)
    {
        _coffeeProcessStatusPublisher = coffeeProcessStatusPublisher;
    }

    [Function(nameof(Dose))]
    public async Task<Coffee> Run([ActivityTrigger] Coffee coffee, FunctionContext executionContext)
    {
        ILogger logger = executionContext.GetLogger(nameof(Dose));

        logger.LogInformation($"Dosing {coffee.Type} coffee  {coffee.Id} for order: {coffee.OrderId} ...");

        await Task.Delay((int)coffee.Intensity * 1000);

        coffee.Status = CoffeeStatus.Dosed;
        coffee.Message = "Dosing Completed";

        logger.LogInformation($"Dosing completed for {coffee.Type} coffee  {coffee.Id} of order: {coffee.OrderId}.");

        await _coffeeProcessStatusPublisher.UpdateCoffeeProcessStatus(coffee);

        return coffee;
    }
}