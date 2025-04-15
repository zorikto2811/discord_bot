using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.IO;

namespace Zorik_Bot
{
    public class Program
    {
        private static DiscordSocketClient client;

        public static async Task Main()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json"), optional: false)
                .AddEnvironmentVariables()
                .Build();

            var config = new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Warning,
                GatewayIntents = GatewayIntents.All,
            };
            var servConfig = new InteractionServiceConfig()
            { 
                LogLevel = LogSeverity.Warning,
                AutoServiceScopes = true,
            };

            client = new DiscordSocketClient(config);

            client.Log += Log;
            client.Ready += Client_Ready;
            client.MessageReceived += HandleCommandAsync;

            var token = configuration["Discord:Token"] ?? Environment.GetEnvironmentVariable("DISCORD_TOKEN");
            
            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("Нет токена");
                return;
            }

            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

            await Task.Delay(-1);
        }

        private static async Task Client_Ready()
        {
            Console.WriteLine("Бот готов");
        }

        private static async Task HandleCommandAsync(SocketMessage messageParam)
        {

            if (!messageParam.Content.StartsWith("!say") ||
               messageParam.Author.IsBot)
               return;
            string cleanMessage = messageParam.Content.Remove(0, 1);
            await messageParam.Channel.SendMessageAsync(cleanMessage);
        }

        private static Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
