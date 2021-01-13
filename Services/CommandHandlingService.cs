using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace SillBot.Services
{
    public class CommandHandlingService
    {
        private readonly CommandService _commands;
        private readonly DiscordSocketClient _discord;
        private readonly IServiceProvider _services;
        private readonly Dictionary<ulong, TaskCompletionSource<SocketReaction>> _reactionAwaiters;
        private readonly Dictionary<ulong, List<IEmote>> _reactionEmotes;

        public CommandHandlingService(IServiceProvider services)
        {
            _commands = services.GetRequiredService<CommandService>();
            _discord = services.GetRequiredService<DiscordSocketClient>();
            _services = services;
            _reactionAwaiters = new Dictionary<ulong, TaskCompletionSource<SocketReaction>>();
            _reactionEmotes = new Dictionary<ulong, List<IEmote>>();

            // Hook CommandExecuted to handle post-command-execution logic.
            _commands.CommandExecuted += CommandExecutedAsync;
            // Hook MessageReceived so we can process each message to see
            // if it qualifies as a command.
            _discord.MessageReceived += MessageReceivedAsync;
            _discord.ReactionAdded += ReactionAddedAsync;
        }

        public async Task InitializeAsync()
        {
            // Register modules that are public and inherit ModuleBase<T>.
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        public async Task MessageReceivedAsync(SocketMessage rawMessage)
        {
            // Ignore system messages, or messages from other bots
            if (!(rawMessage is SocketUserMessage message)) return;
            if (message.Source != MessageSource.User) return;

            // This value holds the offset where the prefix ends
            var argPos = 0;
            // Perform prefix check. You may want to replace this with
            // (!message.HasCharPrefix('!', ref argPos))
            // for a more traditional command format like !help.
            if (!message.HasStringPrefix("Sill, ", ref argPos)) return;

            var context = new SocketCommandContext(_discord, message);
            // Perform the execution of the command. In this method,
            // the command service will perform precondition and parsing check
            // then execute the command if one is matched.
            // TODO Is there no way to use await here without blocking other commands?
            _commands.ExecuteAsync(context, argPos, _services);
            
            // Note that normally a result will be returned by this format, but here
            // we will handle the result in CommandExecutedAsync,
        }

        public async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            // command is unspecified when there was a search failure (command not found); we don't care about these errors
            if (!command.IsSpecified)
                return;

            // the command was successful, we don't care about this result, unless we want to log that a command succeeded.
            if (result.IsSuccess)
                return;

            // the command failed, let's notify the user that something happened.
            await context.Channel.SendMessageAsync($"error: {result.Error}");
            await context.Channel.SendMessageAsync($"error: {result.ErrorReason}");
        }

        public async Task ReactionAddedAsync(Cacheable<IUserMessage, ulong> cachedMessage,
            ISocketMessageChannel originChannel, SocketReaction reaction)
        {
            var message = await cachedMessage.GetOrDownloadAsync();
            if (message != null && reaction.User.IsSpecified)
            {
                Console.WriteLine(
                    $"{reaction.User.Value} just added a reaction '{reaction.Emote}' " +
                    $"to {message.Author}'s message ({message.Id}).");
                
                Console.WriteLine($"{reaction.UserId} - {_discord.CurrentUser.Id}");

                if (_reactionAwaiters.ContainsKey(message.Id) && reaction.UserId != _discord.CurrentUser.Id)
                {
                    Console.WriteLine($"Found awaited message");
                    var emotes = _reactionEmotes[message.Id];
                    if (emotes.Contains(reaction.Emote))
                    {
                        _reactionAwaiters[message.Id].SetResult(reaction);
                        _reactionAwaiters.Remove(message.Id);
                    }
                }
            }
        }

        public Task<SocketReaction> AddReactionAwaiter(ulong Id, List<IEmote> emotes)
        {
            var tcs = new TaskCompletionSource<SocketReaction>();
            _reactionAwaiters.Add(Id, tcs);
            _reactionEmotes.Add(Id, emotes);
            return tcs.Task;
        }
    }
}