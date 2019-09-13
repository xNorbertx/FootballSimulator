using System;
using System.Collections.Generic;

namespace Simulator.Core.Model
{
    public class MatchDay
    {
        public int Id { get; set; }

        public int LeagueId { get; set; }
        public League League { get; set; }
        public ICollection<Match> Matches { get; set; }
    }
}
