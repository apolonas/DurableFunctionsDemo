namespace DurableFunctionsDemo.Common;

public static class Enums
{
    public enum CoffeeType
    {
        Espresso = 1,
        Cappuccino = 2,
        Latte = 3
    }

    public enum Intensity
    {
        Single = 1,
        Double = 2,
        Tripple = 3,
        Quad = 4
    }

    public enum Sweetness
    {
        None = 0,
        Low = 1,
        Medium = 2,
        Sweet = 3
    }

    public enum CoffeeStatus
    {
        Started = 0,
        Ground = 1,
        Dosed = 2,
        Tamped = 3,
        Brewed = 4,
        Failed = 5,
    }

    public enum OrderStatus
    {
        Placed = 0,
        Started = 1,
        PaymentReceived = 2,
        Completed = 3,
        Failed = 4,
        Cancelled = 5
    }

    public enum PaymentStatus
    {
        Requested = 1,
        Approved = 2,
        Failed = 3
    }
}
