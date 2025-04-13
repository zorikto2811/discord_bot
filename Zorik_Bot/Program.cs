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
            client = new DiscordSocketClient();
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

            client.Log += Log;
            client.MessageReceived += HandleCommandAsync;
            await commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(),
                                services: null);

            var token = "MTM2MDA5Nzg4NjE4Mzg4Njk2OQ.Gp0PP_._r-qG0j2db0Ib-bdhxAQWPBqxyZ1FS32jHX-xE";

            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

            await Task.Delay(-1);
        }

        private static async Task HandleCommandAsync(SocketMessage messageParam)
        {
            var message = messageParam as SocketUserMessage;
            if (message == null) return;

            int argPos = 0;

            if (!(message.HasCharPrefix('!', ref argPos) ||
                message.HasMentionPrefix(client.CurrentUser, ref argPos)) ||
                message.Author.IsBot)
                return;

            var context = new SocketCommandContext(client, message);

            await commands.ExecuteAsync(
                context: context,
                argPos: argPos,
                services: null);
        }

        private static Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
