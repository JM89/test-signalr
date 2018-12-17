using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using SignalR_Testing.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SignalR_Testing
{
    public class BackgroundMessageSender : BackgroundService
    {
        private IHubContext<ChatHub> _chatHubCtx;
        private IHubContext<NotificationHub> _hubCtx;

        public BackgroundMessageSender(IHubContext<ChatHub> chatHubCtx, IHubContext<NotificationHub> hubCtx)
        {
            _chatHubCtx = chatHubCtx;
            _hubCtx = hubCtx;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await _chatHubCtx.Clients.All.SendAsync("ReceivedMessageFromServer", "Chat: Message " + Guid.NewGuid());

                    // Send directly a message to a different channel
                    await _hubCtx.Clients.All.SendAsync("ReceivedMessageFromServer", "ServerToClient: Message for all" + Guid.NewGuid());
                    await _hubCtx.Clients.Group("Grp1").SendAsync("ReceivedMessageFromServer", "ServerToClient: Grp 1 message " + Guid.NewGuid());
                    await _hubCtx.Clients.Group("Grp2").SendAsync("ReceivedMessageFromServer", "ServerToClient: Grp 2 message " + Guid.NewGuid());
                    Thread.Sleep(5000);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
