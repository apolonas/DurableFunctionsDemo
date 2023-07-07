using DurableFunctionsDemo.Common.Models;

namespace DurableFunctionsDemo.Common.SignalR;

public interface ICoffeeShopHubClient
{
    Task UpdateCoffeeMakerStatus(Coffee coffee);

    Task UpdateCoffeeProcessStatus(Coffee coffee);

    Task UpdateCoffeeOrderStatus(CoffeeOrder coffeeOrder);
}
