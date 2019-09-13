using System;
using System.Collections.Generic;

namespace Simulator.Core.Model
{
    public class Match
    {
        public int Id { get; set; }

        public int MatchDayId { get; set; }
        public int TeamOneId { get; set; }
        public Team TeamOne { get; set; }
        public int TeamTwoId { get; set; }
        public Team TeamTwo { get; set; }
                    
        public ICollection<Goal> Goals { get; set; }
        public ICollection<Card> Cards { get; set; }
    }
}
