using System;
using System.Collections.Generic;

namespace GameBasicsSimulator.Model
{
    public class Match
    {
        public int Id { get; set; }

        public int MatchDayId { get; set; }
        public ICollection<Team> Teams { get; set; }
        public ICollection<Goal> Goals { get; set; }
        public ICollection<Card> Cards { get; set; }
    }
}
