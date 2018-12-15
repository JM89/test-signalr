using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalR_Testing.Hubs
{
    public static class UserHandler
    {
        public static HashSet<string> ConnectedIds = new HashSet<string>();
    }

    public class ChatHub : Hub
    {
        /// <summary>
        /// Called by the UI directly to broadcast other messages to the UI.
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public async Task SendMessages(string msg)
        {
            if (UserHandler.ConnectedIds.Any())
            {
                await Clients.All.SendAsync("ReceiveMessage", msg);
            }
        }

        /// <summary>
        /// Called by the UI directly to trigger some actions.
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public Task TriggerAction(string cmd)
        {
            Console.WriteLine("Command Received" + cmd);
            return Task.CompletedTask;
        }

        public async override Task OnConnectedAsync()
        {
            UserHandler.ConnectedIds.Add(Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public async override Task OnDisconnectedAsync(Exception ex)
        {
            UserHandler.ConnectedIds.Remove(Context.ConnectionId);
            await base.OnDisconnectedAsync(ex);
        }
    }
}
