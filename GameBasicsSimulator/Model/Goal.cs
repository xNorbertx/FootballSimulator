using System;
using System.ComponentModel.DataAnnotations;

namespace GameBasicsSimulator.Model
{
    public class Goal
    {
        public int Id { get; set; }
        public int Minute { get; set; }

        public int TeamId { get; set; }
        [Required]
        public Team Team { get; set; }

        public int OpponentId { get; set; }
        [Required]
        public Team Opponent { get; set; }

        public int MatchId { get; set;}
        [Required]
        public Match Match { get; set; }
    }
}
