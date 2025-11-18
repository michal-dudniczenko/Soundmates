using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Soundmates.Infrastructure.SignalRHub;

[Authorize]
public class EventHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var userId = Context.User!.FindFirst("sub")!.Value;
        Console.WriteLine($"connected userId: {userId}");
        await Groups.AddToGroupAsync(Context.ConnectionId, userId);
        await base.OnConnectedAsync();
    }
}
