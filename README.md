# Test project for Signal R integration

References
[Tutorial: Get started with ASP.NET Core SignalR](https://docs.microsoft.com/en-us/aspnet/core/tutorials/signalr?view=aspnetcore-2.2&tabs=visual-studio)

## Basic Setup 

1) Create .NET Core 2.2 Web Application 
2) Add Microsoft.AspNetCore.App nuget package
3) Add signalr.js libraries (LibMan) and include the scripts to the index page

```javascript
<script src="~/lib/signalr/dist/browser/signalr.js"></script>
<script src="~/js/chat.js"></script>
```

4) Create chat.js file into the js folder

5) Create a background service for sending regularly messages to the UI

```csharp
public class BackgroundMessageSender : BackgroundService
{
    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Sending msgs
                Thread.Sleep(5000);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
```

6) Add background service to startup.cs

```csharp
public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        //services.Configure...
        services.AddHostedService<BackgroundMessageSender>();
    }
}
```

## Listening messages sent by server from the UI

### Server setup

1) Create a Hub class with no content

```csharp
public class ServerToClientHub : Hub
{

}
```

2) Setup the Startup.cs for signal R

```csharp
public void ConfigureServices(IServiceCollection services)
{
    //...
    services.AddSignalR();
    services.AddHostedService<BackgroundMessageSender>();
}
```

```csharp
public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
    //...
    app.UseSignalR(routes =>
    {
        routes.MapHub<ServerToClientHub>("/serverToClientHub"); // define an explicit route for the UI client
    });
}
```

3) Send messages from background service

```csharp
 public class BackgroundMessageSender : BackgroundService
    {
        private IHubContext<ServerToClientHub> _hubCtx;

        public BackgroundMessageSender(IHubContext<ServerToClientHub> hubCtx)
        {
            _hubCtx = hubCtx;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Send directly a message to a different channel
                    await _hubCtx.Clients.All.SendAsync("ReceivedMessageFromServer", "ServerToClient: Message for all");

                    Thread.Sleep(5000);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
```

### UI client setup

Add this code to chat.js:

```javascript
var serverToClientHub = new signalR.HubConnectionBuilder().withUrl("/serverToClientHub").build();

serverToClientHub.start();
serverToClientHub.on("ReceivedMessageFromServer", function (data) {
    console.log(data);
});

```

## Sending messages from UI to the server

### Requirements

- Hub class is created and startup.cs is setup

### UI Client setup

1) Add a button in index.cshtml

```html
<input type="button" id="triggerAction" value="Trigger actions into the server" />
```

2) Add this code to chat.js:

```javascript
var serverToClientHub = new signalR.HubConnectionBuilder().withUrl("/serverToClientHub").build();
serverToClientHub.start();

document.getElementById("triggerAction").addEventListener("click", function (event) {
    serverToClientHub.invoke("TriggerAction", "exec").catch(function (err) {
        return console.error(err.toString());
    });
});
```

### Server setup

```csharp
public class ServerToClientHub : Hub
{
  public Task TriggerAction(string cmd)
  {
    Console.WriteLine("Command Received" + cmd);
    return Task.CompletedTask;
  }
}
```

Inside this method, you can do whatever you want such as:
- broadcast more messages for more other UI clients
```csharp
await Clients.All.SendAsync("ReceiveMessage", msg)
```
- join a specific message group 
```csharp
await Groups.AddToGroupAsync(Context.ConnectionId, groupname)
```
