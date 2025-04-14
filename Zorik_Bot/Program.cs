using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace Zorik_Bot
{
    public class Program
    {
        private static DiscordSocketClient client;
        private static CommandService commands;

        public static async Task Main()
        {
            commands = new CommandService();

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

            var token = "MTM1OTIwMDI1MjY0OTAxMzUyOQ.GqpP1K.3zAINnJB8Okf1n7_nCi9XYb-64j2S4LingQsY8";

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

            if (!messageParam.Content.StartsWith("!") ||
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
