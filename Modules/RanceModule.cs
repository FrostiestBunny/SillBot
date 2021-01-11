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
            var factions = RanceService.AssignFactions(users, false);

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
    }
}
