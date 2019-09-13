using System;
using System.Collections.Generic;

namespace Simulator.Core.Model
{
    public class League
    {
        public int Id { get; set; }

        public ICollection<MatchDay> Matchdays { get; set; }
    }
}
