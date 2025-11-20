using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Soundmates.Infrastructure.SignalRHub;

[Authorize]
public class EventHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        // 1. Retrieve the User ID safely.
        // Note: By default, Microsoft maps the "sub" claim to ClaimTypes.NameIdentifier.
        var userId = Context.User?.FindFirst("sub")?.Value ?? 
                     Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        // 2. Validate the ID exists before using it.
        if (string.IsNullOrEmpty(userId))
        {
            // Log the issue and abort. This prevents the crash.
            Console.WriteLine($"[Error] Connection attempted without valid 'sub' or 'NameIdentifier' claim. User Identity: {Context.User?.Identity?.Name ?? "Unknown"}");
            Context.Abort(); 
            return;
        }

        Console.WriteLine($"connected userId: {userId}");
        await Groups.AddToGroupAsync(Context.ConnectionId, userId);
        await base.OnConnectedAsync();
    }
}