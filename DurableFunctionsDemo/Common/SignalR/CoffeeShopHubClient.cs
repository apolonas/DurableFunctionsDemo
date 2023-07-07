using DurableFunctionsDemo.Common.Models;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;

namespace DurableFunctionsDemo.Common.SignalR;

public class CoffeeShopHubClient : SignalRClientBase, ICoffeeShopHubClient
{
    private readonly IConfiguration _configuration;

    public CoffeeShopHubClient(IConfiguration configuration) : base(configuration["SignalRHubAddress"])
    {
        _configuration = configuration;
    }

    public async Task UpdateCoffeeMakerStatus(Coffee coffee)
    {
        try
        {
            await Start();
            await HubConnection.SendAsync(nameof(UpdateCoffeeMakerStatus), coffee.Id, coffee.OrderId, coffee.Message, (int)coffee.Status);
        }
        catch (Exception ex)
        {
            //Console.WriteLine(ex.Message);
        }   
    }

    public async Task UpdateCoffeeProcessStatus(Coffee coffee)
    {
        try
        {
            await Start();
            await HubConnection.SendAsync(nameof(UpdateCoffeeProcessStatus), coffee.Id, coffee.OrderId, coffee.Message, (int)coffee.Status);
        }
        catch (Exception ex)
        {
            //Console.WriteLine(ex.Message);
        }
    }

    public async Task UpdateCoffeeOrderStatus(CoffeeOrder coffeeOrder)
    {
        try
        {
            await Start();
            await HubConnection.SendAsync(nameof(UpdateCoffeeOrderStatus), coffeeOrder.Id, coffeeOrder.Message, (int)coffeeOrder.Status);
        }
        catch (Exception ex)
        {
            //Console.WriteLine(ex.Message);
        }
    }
}
