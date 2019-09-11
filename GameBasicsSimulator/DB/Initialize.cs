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

                League league = CreateLeagueMatches(teams.ToList());

                context.Leagues.Add(league);

                context.SaveChanges();
            }
        }

        //Algorithm comes from: https://blog.moove-it.com/algorithm-generation-soccer-fixture/
        private static League CreateLeagueMatches(List<Team> teams)
        {
            var league = new League();
            league.Matchdays = new List<MatchDay>();
            var lastTeam = teams.Last();

            for (var i = 1; i < teams.Count; i++)
            {
                _ = new MatchDay();
                MatchDay matchDay;

                if (i == 1)
                {
                    matchDay = CreateMatchDayOne(teams, lastTeam);
                }
                else
                {
                    matchDay = CreateMatchDay(teams, lastTeam);
                }

                teams.CreateNewOrder(matchDay);
                league.Matchdays.Add(matchDay);
            }
            return league;
        }


        private static MatchDay CreateMatchDayOne(List<Team> teams, Team lastTeam)
        {

            var matchDay = new MatchDay();
            matchDay.Matches = new List<Match>();
            var match = new Match();

            for (var i = 0; i < teams.Count; i++)
            {
                //Add teams
                if (i == 0)
                {
                    match.TeamOne = lastTeam;
                    continue;
                }

                //Add match to matchday
                if (i % 2 == 1)
                {
                    match.TeamTwo = teams[i - 1];
                    matchDay.Matches.Add(match);
                    match = new Match();
                }
                else
                {
                    match.TeamOne = teams[i - 1];
                }
            }
            return matchDay;
        }

        private static MatchDay CreateMatchDay(List<Team> teams, Team lastTeam)
        {
            var matchDay = new MatchDay();
            matchDay.Matches = new List<Match>();
            var match = new Match();
            var len = teams.Count;

            for (var i = 0; i < len; i++)
            {
                //Get teams to add to match
                if (i == 0)
                {
                    match.TeamOne = lastTeam;
                    teams.Remove(lastTeam);
                    continue;
                }


                if (i == 1)
                {
                    match.TeamTwo = teams.Last();
                    matchDay.Matches.Add(match);
                    match = new Match();
                    continue;
                }

                var firstTeam = teams.First();

                //Add match to matchday
                if (i % 2 == 1)
                {
                    match.TeamTwo = firstTeam;
                    matchDay.Matches.Add(match);
                    match = new Match();
                }
                else
                {
                    match.TeamOne = firstTeam;
                }

                teams.Remove(firstTeam);

            }

            return matchDay;
        }

        private static List<Team> CreateNewOrder(this List<Team> teams, MatchDay matchDay)
        {
            teams.RemoveAll(x => true);
            var iterableList = matchDay.Matches.ToList();

            for (var i = 0; i < iterableList.Count; i++)
            {
                teams.Add(iterableList[i].TeamOne);
                teams.Add(iterableList[i].TeamTwo);
            }

            return teams;
        }
    } 
}
