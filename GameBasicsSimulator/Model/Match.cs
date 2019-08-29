using System;
using System.Collections.Generic;

namespace GameBasicsSimulator.Model
{
    public class Match
    {
        public int Id { get; set; }

        public int MatchDayId { get; set; }
        public List<MatchTeam> MatchTeams { get; set; }
        public List<Goal> Goals { get; set; }
        public List<Card> Cards { get; set; }
    }
}
