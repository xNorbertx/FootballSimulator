using System;
using System.Collections.Generic;
using System.Linq;
using GameBasicsSimulator.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GameBasicsSimulator.DB
{
    public class Initialize
    {
        public static void InitializeDb(IServiceProvider serviceProvider)
        {
            using (var context = new SimulatorContext(
                serviceProvider.GetRequiredService<DbContextOptions<SimulatorContext>>()))
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

                List<Match> matches = new List<Match>();                

                context.Teams.AddRange(teams);

                context.SaveChanges();
            }
        }
    }
}