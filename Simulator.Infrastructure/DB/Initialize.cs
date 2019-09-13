using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Simulator.Core.Model;
using Simulator.Engine.Algorithms;

namespace Simulator.Infrastructure.DB
{
    public static class Initialize
    {
        public static void InitializeDb(SimulatorContext context)
        {
            if (context.Teams.Any())
            {
                return; //Data was already seeded
            }

            List<Team> teams = new List<Team> {
                    new Team { Id = 1, Name = "Ajax", Points = 0, Strength = 90, Morale = 80 },
                    new Team { Id = 2, Name = "PSV", Points = 0, Strength = 88, Morale = 80 },
                    new Team { Id = 3, Name = "Feyenoord", Points = 0, Strength = 78, Morale = 80 },
                    new Team { Id = 4, Name = "AZ", Points = 0, Strength = 75, Morale = 80 }
                };

            context.Teams.AddRange(teams);

            if (context.Leagues.Any())
            {
                context.SaveChanges();
                return; //We already have a league
            }

            League league = teams.CreateLeagueMatches();

            context.Leagues.Add(league);

            context.SaveChanges();
        }        
    }
}
