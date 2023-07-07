using static DurableFunctionsDemo.Common.Enums;

namespace DurableFunctionsDemo.Patterns.HumanInteraction;

public class ProcessPaymentEventModel
{
    public PaymentStatus PaymentStatus { get; set; }
}
