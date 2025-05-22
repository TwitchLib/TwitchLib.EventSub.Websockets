using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using TwitchLib.EventSub.Websockets.Extensions;

namespace TwitchLib.EventSub.Websockets.Example.NetStandard
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

            builder.Configuration.AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json"));

            //Add services to the container.
            builder.Services.AddLogging();
            builder.Services.AddTwitchLibEventSubWebsockets();
            builder.Services.AddHostedService<WebsocketHostedService>();

            var app = builder.Build();

            app.Run();
        }
    }
}