using Microsoft.AspNetCore.SignalR;

namespace Api.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task NewNotification(object content)
            => await Clients.Others.SendAsync("NewNotification", content);
    }
}
