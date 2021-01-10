using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using SillBot.Services;
using SillBot.Utils;

namespace SillBot.Modules
{
    // Modules must be public and inherit from an IModuleBase
    public class PublicModule : ModuleBase<SocketCommandContext>
    {
        // Dependency Injection will fill this value in for us
        public PictureService PictureService { get; set; }

        [Command("ping")]
        [Alias("pong", "hello")]
        public Task PingAsync()
            => ReplyAsync("pong!");

        [Command("hit")]
        public Task HitAsync()
            => ReplyAsync("Ouch! Pls no bully");

        [Command("cat")]
        public async Task CatAsync()
        {
            // Get a stream containing an image of a cat
            var stream = await PictureService.GetCatPictureAsync();
            // Streams must be seeked to their beginning before being uploaded!
            stream.Seek(0, SeekOrigin.Begin);
            await Context.Channel.SendFileAsync(stream, "cat.png");
        }

        // Get info on a user, or the user who invoked the command if one is not specified
        [Command("userinfo")]
        public async Task UserInfoAsync(IUser user = null)
        {
            user = user ?? Context.User;

            await ReplyAsync(user.ToString());
        }

        // Ban a user
        [Command("ban")]
        [RequireContext(ContextType.Guild)]
        // make sure the user invoking the command can ban
        [RequireUserPermission(GuildPermission.BanMembers)]
        // make sure the bot itself can ban
        [RequireBotPermission(GuildPermission.BanMembers)]
        public async Task BanUserAsync(IGuildUser user, [Remainder] string reason = null)
        {
            await user.Guild.AddBanAsync(user, reason: reason);
            await ReplyAsync("ok!");
        }

        // [Remainder] takes the rest of the command's arguments as one argument, rather than splitting every space
        [Command("echo")]
        public Task EchoAsync([Remainder] string text)
            // Insert a ZWSP before the text to prevent triggering other bots!
            => ReplyAsync('\u200B' + text);

        // 'params' will parse space-separated elements into a list
        [Command("list")]
        public Task ListAsync(params string[] objects)
            => ReplyAsync("You listed: " + string.Join("; ", objects));

        // Setting a custom ErrorMessage property will help clarify the precondition error
        [Command("guild_only")]
        [RequireContext(ContextType.Guild, ErrorMessage = "Sorry, this command must be ran from within a server, not a DM!")]
        public Task GuildOnlyCommand()
            => ReplyAsync("Nothing to see here!");

        [Command("users")]
        [RequireContext(ContextType.Guild)]
        public async Task UsersAsync()
        {
            var response = "";
            var guild = Context.Guild;
            foreach (var user in guild.Users)
            {
                response += user.Username + " ";
            }
            await ReplyAsync(response);
        }

        [Command("teams")]
        [RequireContext(ContextType.Guild)]
        public async Task TeamsAsync(int memberCount)
        {
            var users = Context.Guild.Users.Where(user => !user.IsBot);
            users = users.Shuffle();
            
            int teamCount = (users.Count() - 1) / memberCount + 1;

            List<List<string>> teams = new List<List<string>>();

            for (int i = 0; i < teamCount; i++)
            {
                teams.Add(new List<string>());
            }

            int counter = 0;
            foreach (var user in users)
            {
                int teamIndex = (counter / memberCount);
                teams[teamIndex].Add(user.Username);
                counter++;
            }

            var response = "";

            counter = 1;
            foreach (var team in teams)
            {
                response += $"Team {counter}: ";
                response += string.Join(", ", team.ToArray());
                response += "\n";
                counter++;
            }

            await ReplyAsync(response);
        }
    }
}