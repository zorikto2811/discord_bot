using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Zorik_Bot
{
    public class Program
    {
        private static DiscordSocketClient client;

        public static async Task Main()
        {
            client = new DiscordSocketClient();
            client.Log += Log;

            var token = "MTM2MDA5Nzg4NjE4Mzg4Njk2OQ.GWSIpm.jikPhvQeQ9N4IngCJlSAJX7Mi8y9wQpawJnXOg";

            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

            

            await Task.Delay(-1);
        }

        private static Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
