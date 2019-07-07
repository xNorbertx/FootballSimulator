using System;
using System.Collections.Generic;

namespace GameBasicsSimulator.Model
{
    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Strength { get; set; }
        public int Morale { get; set; }
        public int Points { get; set; }

        public List<Goal> Goals { get; set; }
        public List<Goal> GoalsConceded { get; set; }
        public List<Card> Cards { get; set; }
        public List<MatchTeam> MatchTeams { get; set; }
    }
}
