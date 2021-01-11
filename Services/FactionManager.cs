using System.Collections.Generic;
using System.Linq;
using SillBot.Models;

namespace SillBot.Services
{
    public class FactionManager
    {
        public List<Faction> Factions { get; private set; }

        public FactionManager()
        {
            Factions = new List<Faction>();
        }

        public void Init()
        {
            Factions.Add(new Faction
            {
                Name = "Leazas",
                Location = Location.North,
                IsFiends = false
            });
            Factions.Add(new Faction
            {
                Name = "Zeth",
                Location = Location.West | Location.South,
                IsFiends = false
            });
            Factions.Add(new Faction
            {
                Name = "Helman",
                Location = Location.North,
                IsFiends = false
            });
            Factions.Add(new Faction
            {
                Name = "Nippon",
                Location = Location.East,
                IsFiends = false
            });
            Factions.Add(new Faction
            {
                Name = "Hornet",
                Location = Location.West | Location.North,
                IsFiends = true
            });
            Factions.Add(new Faction
            {
                Name = "Kayblis",
                Location = Location.West | Location.South,
                IsFiends = true
            });
            Factions.Add(new Faction
            {
                Name = "Rebels",
                Location = Location.North | Location.East,
                IsFiends = false
            });
            Factions.Add(new Faction
            {
                Name = "Pentagon",
                Location = Location.West | Location.South,
                IsFiends = false
            });
            Factions.Add(new Faction
            {
                Name = "Patton",
                Location = Location.West | Location.North,
                IsFiends = false
            });
            Factions.Add(new Faction
            {
                Name = "Kalar",
                Location = Location.West,
                IsFiends = false
            });
            Factions.Add(new Faction
            {
                Name = "Shangri-La",
                Location = Location.West,
                IsFiends = false
            });
            Factions.Add(new Faction
            {
                Name = "Custom",
                Location = Location.East | Location.South,
                IsFiends = false
            });
            Factions.Add(new Faction
            {
                Name = "Rance",
                Location = Location.North,
                IsFiends = false
            });
            Factions.Add(new Faction
            {
                Name = "AL Church",
                Location = Location.South,
                IsFiends = false
            });
            Factions.Add(new Faction
            {
                Name = "The O",
                Location = Location.East,
                IsFiends = false
            });
            Factions.Add(new Faction
            {
                Name = "Portugal",
                Location = Location.East | Location.South,
                IsFiends = false
            });
            Factions.Add(new Faction
            {
                Name = "Ice",
                Location = Location.South,
                IsFiends = false
            });
            Factions.Add(new Faction
            {
                Name = "Red",
                Location = Location.South,
                IsFiends = false
            });
        }
    }
}
