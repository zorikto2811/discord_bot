namespace Zorik_Bot
{
    using Discord;
    using Discord.WebSocket;
    using Discord.Interactions;
    using System;
    using System.Threading.Tasks;
    using System.Linq;

    namespace Zorik_Bot
    {
        public class RoomManagementModule : InteractionModuleBase<SocketInteractionContext>
        {
            [SlashCommand("room", "Управление приватной комнатой")]
            public async Task RoomManagement()
            {
                if (!(Context.User is SocketGuildUser user))
                {
                    await RespondAsync("Эта команда может быть использована только на сервере.", ephemeral: true);
                    return;
                }

                var privateChannel = user.VoiceChannel;
                if (privateChannel == null || !privateChannel.Name.StartsWith("🔒"))
                {
                    await RespondAsync("У вас нет приватной комнаты. Создайте её, зайдя в голосовой канал и используя команду /create-room", ephemeral: true);
                    return;
                }

                var builder = new ComponentBuilder()
                    .WithButton("Показать комнату", "show_room", ButtonStyle.Primary)
                    .WithButton("Изменить название", "change_name", ButtonStyle.Secondary)
                    .WithButton("Передать управление", "transfer_ownership", ButtonStyle.Secondary)
                    .WithButton("Кикнуть пользователя", "kick_user", ButtonStyle.Danger)
                    .WithButton("Выдать доступ", "grant_access", ButtonStyle.Success)
                    .WithButton("Забрать доступ", "revoke_access", ButtonStyle.Danger)
                    .WithButton("Выдать право говорить", "grant_speak", ButtonStyle.Success)
                    .WithButton("Изменить лимит", "change_limit", ButtonStyle.Secondary)
                    .WithButton("Открыть/Закрыть комнату", "toggle_privacy", ButtonStyle.Secondary);

                await RespondAsync("Управление комнатой:", components: builder.Build(), ephemeral: true);
            }

            [ComponentInteraction("show_room")]
            public async Task ShowRoom()
            {
                var user = Context.User as SocketGuildUser;
                var channel = user?.VoiceChannel;

                if (channel == null)
                {
                    await RespondAsync("Вы не находитесь в голосовом канале.", ephemeral: true);
                    return;
                }

                var embed = new EmbedBuilder()
                    .WithTitle($"Информация о комнате {channel.Name}")
                    .AddField("ID канала", channel.Id)
                    .AddField("Создатель", channel.GetUser((ulong)channel.CategoryId)?.Mention ?? "Неизвестно")
                    .AddField("Лимит пользователей", channel.UserLimit == 0 ? "Без лимита" : channel.UserLimit.ToString())
                    .AddField("Текущее количество пользователей", channel.Users.Count)
                    .WithColor(Color.Blue);

                await RespondAsync(embed: embed.Build(), ephemeral: true);
            }

            public class LimitModal : IModal
            {
                public string Title => "Изменение лимита пользователей";
                [InputLabel("Новый лимит пользователей")]
                [ModalTextInput("user_limit", placeholder: "Введите число от 0 до 99 (0 = без лимита)", minLength: 1, maxLength: 2)]
                public string UserLimit { get; set; }
            }

            [ComponentInteraction("change_limit")]
            public async Task ChangeLimit()
            {
                var user = Context.User as SocketGuildUser;
                var channel = user?.VoiceChannel;

                if (channel == null)
                {
                    await RespondAsync("Вы не находитесь в голосовом канале.", ephemeral: true);
                    return;
                }

                await RespondWithModalAsync<LimitModal>("limit_modal");
            }

            [ModalInteraction("limit_modal")]
            public async Task HandleLimitModal(LimitModal modal)
            {
                var user = Context.User as SocketGuildUser;
                var channel = user?.VoiceChannel;

                if (channel == null)
                {
                    await RespondAsync("Вы не находитесь в голосовом канале.", ephemeral: true);
                    return;
                }

                if (!int.TryParse(modal.UserLimit, out int limit))
                {
                    await RespondAsync("Пожалуйста, введите корректное число.", ephemeral: true);
                    return;
                }

                if (limit < 0 || limit > 99)
                {
                    await RespondAsync("Лимит пользователей должен быть от 0 до 99.", ephemeral: true);
                    return;
                }

                await channel.ModifyAsync(x => x.UserLimit = limit);
                await RespondAsync($"Лимит пользователей изменен на {(limit == 0 ? "без лимита" : limit.ToString())}.", ephemeral: true);
            }

            [ComponentInteraction("toggle_privacy")]
            public async Task TogglePrivacy()
            {
                var user = Context.User as SocketGuildUser;
                var channel = user?.VoiceChannel;

                if (channel == null)
                {
                    await RespondAsync("Вы не находитесь в голосовом канале.", ephemeral: true);
                    return;
                }

                bool isPrivate = channel.Name.StartsWith("🔒");
                string newName = isPrivate ? channel.Name.Substring(2) : "🔒" + channel.Name;
                
                await channel.ModifyAsync(x => x.Name = newName);
                await RespondAsync($"Комната {(isPrivate ? "открыта" : "закрыта")}.", ephemeral: true);
            }
        }
    }
}