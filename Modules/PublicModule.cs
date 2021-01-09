using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using SillBot.Services;

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
        
        [Command("santa")]
        [RequireContext(ContextType.Guild)]
        [RequireOwner()]
        public async Task SecretSantaAsync(params IGuildUser[] users) {
            foreach(var u in users)
            {
                Console.WriteLine(u.ToString());
            }

            var lots = new List<IGuildUser>(users);
            var userNames = "";
            foreach (IGuildUser u in users)
            {
                var name = string.IsNullOrEmpty(u.Nickname) ? u.Username : u.Nickname;
                userNames += name + " ";
            }
            await ReplyAsync("Starting Secret Santa with the following users:\n" + userNames);

            foreach (IGuildUser u in users)
            {
                var result = Draw(u, lots);
                await u.SendMessageAsync($"You drew {result.Username}.");
            }
            await ReplyAsync("Secret Santa drawing finished.");
        }

        private IGuildUser Draw(IGuildUser drawer, List<IGuildUser> lots)
        {
            Random rnd = new Random();
            int index = rnd.Next(lots.Count);
            var pick = lots[index];
            while (pick.Id == drawer.Id)
            {
                index = rnd.Next(lots.Count);
                pick = lots[index];
            }
            lots.RemoveAt(index);
            return pick;
        }
    }
}