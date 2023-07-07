namespace DurableFunctionsDemo.Common.SignalR;
public interface ISignalRClient
{
    bool IsConnected { get; }

    Task Start();
}
