﻿using System;

namespace GameBasicsSimulator.Model
{
    public class Goal
    {
        public int Id { get; set; }
        public int Minute { get; set; }

        public int TeamId { get; set; }
        public Team Team { get; set; }

        public int OpponentId { get; set; }
        public Team Opponent { get; set; }

        public int MatchId { get; set;}
        public Match Match { get; set; }
    }
}
