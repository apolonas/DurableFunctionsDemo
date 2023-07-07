using DurableFunctionsDemo.Common.Models;
using static DurableFunctionsDemo.Common.Enums;

namespace DurableFunctionsDemo.Common;
public static class CoffeePriceCalculator
{
    public static decimal CalculateOrderPrice(CoffeeOrder coffeeOrder)
    {
        decimal price = 0m;

        foreach (var coffee in coffeeOrder.Coffees)
        {
            price += CoffeeTypePrice(coffee.Type) + IntensityPrice(coffee.Intensity) + SweetnessPrice(coffee.Sweetness);
        }

        return price;
    }

    public static decimal CalculateCoffeePrice(Coffee coffee)
    {
        decimal price = CoffeeTypePrice(coffee.Type) + IntensityPrice(coffee.Intensity) + SweetnessPrice(coffee.Sweetness);

        return price;
    }

    private static decimal CoffeeTypePrice(CoffeeType coffeeType)
    {
        return coffeeType switch
        {
            CoffeeType.Espresso => 1m,
            CoffeeType.Cappuccino => 2m,
            CoffeeType.Latte => 4m,
            _ => 1m,
        };
    }

    private static decimal IntensityPrice(Intensity intensity)
    {
        return intensity switch
        {
            Intensity.Single => 2m,
            Intensity.Double => 4m,
            Intensity.Tripple => 6m,
            Intensity.Quad => 8m,
            _ => 2m,
        };
    }

    private static decimal SweetnessPrice(Sweetness sweetness)
    {
        return sweetness switch
        {
            Sweetness.None => 0m,
            Sweetness.Low => 1m,
            Sweetness.Medium => 2m,
            Sweetness.Sweet => 3m,
            _ => 0m,
        };
    }
}
