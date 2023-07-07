using static DurableFunctionsDemo.Common.Enums;

namespace DurableFunctionsDemo.Common.Models;

public class Coffee
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public CoffeeType Type { get; set; }

    public Intensity Intensity { get; set; }

    public Sweetness Sweetness { get; set; }

    public CoffeeStatus Status { get; set; }

    public string Message { get; set; }
}
