using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using Zorik_Bot.Zorik_Bot;

namespace Zorik_Bot
{
    public class Program
    {
        private static DiscordSocketClient client;
        private static InteractionService interactions;
        private static IServiceProvider services;

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

            // Setup services
            services = new ServiceCollection()
                .AddSingleton(client = new DiscordSocketClient(config))
                .AddSingleton(interactions = new InteractionService(client, servConfig))
                .BuildServiceProvider();

            // Register modules
            await interactions.AddModuleAsync<RoomManagementModule>(services);

            client.Log += Log;
            client.Ready += Client_Ready;
            client.InteractionCreated += HandleInteraction;

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
            await interactions.RegisterCommandsGloballyAsync();
        }

        private static async Task HandleInteraction(SocketInteraction interaction)
        {
            try
            {
                var context = new SocketInteractionContext(client, interaction);
                var result = await interactions.ExecuteCommandAsync(context, services);
                
                if (!result.IsSuccess)
                {
                    Console.WriteLine($"Error executing command: {result.ErrorReason}");
                    if (interaction.Type == InteractionType.ApplicationCommand)
                    {
                        await interaction.RespondAsync("Произошла ошибка при выполнении команды.", ephemeral: true);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                if (interaction.Type == InteractionType.ApplicationCommand)
                {
                    await interaction.RespondAsync("Произошла ошибка при выполнении команды.", ephemeral: true);
                }
            }
        }

        private static Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
