using Nasa.Dashboard.Clients.Contracts;

namespace Nasa.Dashboard.Reducers.Internal.System;

internal interface ISystemService
{
    Task<bool> PingAsync();
}

internal class SystemService(IPathfinderClient client) : ISystemService
{
    public async Task<bool> PingAsync()
    {
        var result = await client.PingAsync();
        return result;
    }
}