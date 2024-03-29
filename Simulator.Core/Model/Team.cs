﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Simulator.Core.Model
{
    public class Team
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public int Strength { get; set; }
        public int Morale { get; set; }
        public int Points { get; set; }

        public ICollection<Match> HomeMatches { get; set; }
        public ICollection<Match> AwayMatches { get; set; }

        public ICollection<Goal> Goals { get; set; }
        public ICollection<Card> Cards { get; set; }
    }
}
