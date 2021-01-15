using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Discord;
using Discord.Commands;
using SillBot.Services;

namespace SillBot.Modules
{
    public class RanceModule : ModuleBase<SocketCommandContext>
    {
        public RanceService RanceService { get; set; }

        [Command("teams")]
        [RequireContext(ContextType.Guild)]
        public async Task TeamsAsync(int memberCount)
        {
            var users = Context.Guild.Users.Where(user => !user.IsBot);
            var teams = RanceService.FormTeams(memberCount, users);

            var response = new EmbedBuilder
            {
                Color = Color.DarkBlue
            };
            response.WithAuthor(Context.Client.CurrentUser)
                .WithCurrentTimestamp();

            int counter = 1;
            foreach (var team in teams)
            {
                response.AddField($"Team {counter}", string.Join(", ", team.ToArray()));
                counter++;
            }

            await ReplyAsync(embed: response.Build());
        }

        [Command("factions")]
        [RequireContext(ContextType.Guild)]
        public async Task FactionsAsync()
        {
            var users = Context.Guild.Users.Where(user => !user.IsBot);

            var embed = new EmbedBuilder
            {
                Description = "Include fiends?",
                Color = Color.DarkGreen
            };
            embed.WithAuthor(Context.Client.CurrentUser)
                .WithCurrentTimestamp();

            var message = await ReplyAsync(embed: embed.Build());
            var yes_emoji = new Emoji("✅");
            var no_emoji = new Emoji("❌");
            var emotes = new List<IEmote>();
            emotes.Add(yes_emoji);
            emotes.Add(no_emoji);
            await message.AddReactionsAsync(emotes.ToArray());

            var reaction = await RanceService.WaitForReactionAsync(message.Id, emotes);
            var addFiends = reaction.Emote.ToString() == yes_emoji.ToString();

            var factions = RanceService.AssignFactions(users, addFiends);

            var response = new EmbedBuilder
            {
                Color = Color.Blue
            };
            response.WithAuthor(Context.Client.CurrentUser)
                .WithCurrentTimestamp();

            var desc = "";

            int counter = 0;
            foreach (var user in users)
            {
                desc += $"{user.Username}: **{factions.ElementAt(counter).Name}**\n";
                counter++;
            }

            response.WithDescription(desc);

            await ReplyAsync(embed: response.Build());
        }

        [Command("logout")]
        [RequireOwner()]
        public async Task LogoutAsync()
        {
            await ReplyAsync("Going to sleep, bye.");
            await RanceService.LogoutAsync();
        }
    }
}
