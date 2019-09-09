using System;
using System.Collections.Generic;
using System.Linq;
using GameBasicsSimulator.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GameBasicsSimulator.DB
{
    public static class Initialize
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

                ICollection<Team> teams = new List<Team> {
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

                List<Match> matches = new List<Match>(); 

                League league = CreateLeagueMatches(teams.ToList());

                context.Leagues.Add(league);

                context.SaveChanges();
            }
        }

        private static League CreateLeagueMatches(List<Team> teams)
        {
            var retval = new League();
            var lastTeam = teams.Last();

            for (var i = 1; i < teams.Count; i++)
            {
                _ = new MatchDay();
                MatchDay matchDay;

                if (i == 1)
                {
                    matchDay = createMatchDayOne(teams, lastTeam);
                }
                else
                {
                    matchDay = createMatchDay(teams, lastTeam);
                }

                teams.createNewOrder(matchDay);
                retval.Matchdays.Add(matchDay);
            }
            return retval;
        }

        private static List<Team> createNewOrder(this List<Team> teams, MatchDay matchDay)
        {
            teams.RemoveAll(x => true);
            var iterableList = matchDay.Matches.ToList();

            for (var i = 0; i < iterableList.Count; i++)
            {
                teams.Add(iterableList[i].Teams.First());
                teams.Add(iterableList[i].Teams.Last());
            }

            return teams;
        }

        private static MatchDay createMatchDayOne(List<Team> teams, Team lastTeam)
        {
            var match = new Match();
            var matchDay = new MatchDay();

            for (var i = 0; i < teams.Count; i++)
            {
                //Add teams
                if (i == 0)
                {
                    match.Teams.Add(lastTeam);
                }
                else
                {
                    match.Teams.Add(teams[i-1]);
                }
                //Add match to matchday
                if (i % 2 == 0 && i > 1)
                {
                    matchDay.Matches.Add(match);
                    match = new Match();
                }
            }
            return matchDay;
        }

        private static MatchDay createMatchDay(List<Team> teams, Team lastTeam)
        {
            var match = new Match();
            var matchDay = new MatchDay();

            for (var i = 0; i < teams.Count; i++)
            {
                //Get teams to add to match
                if (i == 0)
                {
                    match.Teams.Add(lastTeam);
                    teams.Remove(lastTeam);
                }
                else if (i == 1)
                {
                    match.Teams.Add(teams.Last());
                }
                else
                {
                    var firstTeam = teams.First();
                    match.Teams.Add(firstTeam);
                    teams.Remove(firstTeam);
                }

                //Add match to matchday
                if (i % 2 == 0 && i > 1)
                {
                    matchDay.Matches.Add(match);
                    match = new Match();
                }
            }

            return matchDay;
        }
    } 
}
