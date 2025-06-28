using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

public class TradeHub : Hub
{
    public async Task SendNotification(string userId, string message)
    {
        await Clients.User(userId).SendAsync("ReceiveNotification", message);
    }
}