using static DurableFunctionsDemo.Common.Enums;

namespace DurableFunctionsDemo.Common.Models;

public class CoffeeOrder
{
    public int Id { get; set; }

    public List<Coffee> Coffees { get; set; } = new List<Coffee>();

    public OrderStatus Status { get; set; }

    public PaymentStatus PaymentStatus { get; set; }

    public string Message { get; set; } 

    public decimal TotalCost { get; set; }
}
