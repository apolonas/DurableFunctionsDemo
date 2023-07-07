using DurableFunctionsDemo.Common.Models;
using Microsoft.Azure.Functions.Worker;

namespace DurableFunctionsDemo.Common.SignalR;

public class MessagePublisher
{
    private readonly ICoffeeShopHubClient _coffeeShopHubClient;

    public MessagePublisher(ICoffeeShopHubClient coffeeShopHubClient)
    {
        _coffeeShopHubClient = coffeeShopHubClient;
    }

    [Function(nameof(UpdateCoffeeMakerStatus))]
    public async Task UpdateCoffeeMakerStatus([ActivityTrigger] Coffee coffee, FunctionContext executionContext)
    {
        await _coffeeShopHubClient.UpdateCoffeeMakerStatus(coffee);
    }

    [Function(nameof(UpdateCoffeeProcessStatus))]
    public async Task UpdateCoffeeProcessStatus([ActivityTrigger] Coffee coffee, FunctionContext executionContext)
    {
        await _coffeeShopHubClient.UpdateCoffeeProcessStatus(coffee);
    }

    [Function(nameof(UpdateCoffeeOrderStatus))]
    public async Task UpdateCoffeeOrderStatus([ActivityTrigger] CoffeeOrder coffeeOrder, FunctionContext executionContext)
    {
        await _coffeeShopHubClient.UpdateCoffeeOrderStatus(coffeeOrder);
    }
}

