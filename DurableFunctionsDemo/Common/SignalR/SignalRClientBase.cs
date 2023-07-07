using Microsoft.AspNetCore.SignalR.Client;

namespace DurableFunctionsDemo.Common.SignalR;
public abstract class SignalRClientBase : ISignalRClient, IAsyncDisposable
{
    protected bool Started { get; private set; }

    protected SignalRClientBase(string hubPath) =>
        HubConnection = new HubConnectionBuilder()
            .WithUrl(hubPath)
            .WithAutomaticReconnect()
            .Build();

    public bool IsConnected =>
        HubConnection.State == HubConnectionState.Connected;

    protected HubConnection HubConnection { get; private set; }

    public async ValueTask DisposeAsync()
    {
        if (HubConnection != null)
        {
            await HubConnection.DisposeAsync();
        }
    }

    public async Task Start()
    {
        if (!Started)
        {
            try
            {
                await HubConnection.StartAsync();
                Started = true;
            }
            catch (Exception ex) 
            {
                //Do nothing. SignalR is not required.
                //Add Logging.
            }
        }
    }
}
