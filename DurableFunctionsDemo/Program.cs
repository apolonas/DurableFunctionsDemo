using DurableFunctionsDemo.Common.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        services.AddScoped<ICoffeeShopHubClient, CoffeeShopHubClient>();
    })
    .Build();

host.Run();
