using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SillBot.Models;
using SillBot.Utils;
using Discord;
using Discord.WebSocket;

namespace SillBot.Services
{
    public class RanceService
    {
        private readonly FactionManager _factionManager;
        private readonly CommandHandlingService _commandHandlingService;

        public RanceService(FactionManager fm, CommandHandlingService chm)
        {
            _factionManager = fm;
            _factionManager.Init();
            _commandHandlingService = chm;
        }

        public List<List<string>> FormTeams(int memberCount, 
            IEnumerable<Discord.WebSocket.SocketGuildUser> users)
        {
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

            return teams;
        }

        public List<Faction> AssignFactions(IEnumerable<Discord.WebSocket.SocketGuildUser> players,
            bool includeFiends)
        {
            var factions = includeFiends ? _factionManager.Factions :
                _factionManager.Factions.Where(faction => !faction.IsFiends);

            factions = factions.Shuffle();
            
            var assignedFactions = factions.Take(players.Count()).ToList();

            return assignedFactions;
        }

        public async Task<SocketReaction> WaitForReactionAsync(ulong Id, List<IEmote> emotes)
        {
            var reaction = await _commandHandlingService.AddReactionAwaiter(Id, emotes);
            return reaction;
        }
    }
}
