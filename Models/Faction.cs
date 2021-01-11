using System;
using System.Collections.Generic;

namespace SillBot.Models
{
    [Flags]
    public enum Location
    {
        North = 1,
        East = 2,
        South = 4,
        West = 8
    }

    public class Faction
    {
        public string Name { get; set; }
        
        public Location Location { get; set; }

        public bool IsFiends { get; set; }
        
        public bool IsTaken { get; set; } = false;
    }
}
