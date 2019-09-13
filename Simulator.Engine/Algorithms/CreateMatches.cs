using System;
using System.Collections.Generic;
using System.Linq;
using Simulator.Core.Model;

namespace Simulator.Engine.Algorithms
{
    //Algorithm comes from: https://blog.moove-it.com/algorithm-generation-soccer-fixture/
    public static class CreateMatches
    {
        public static League CreateLeagueMatches(this List<Team> teams)
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
                if (i == 0)
                {
                    match.TeamOne = lastTeam;
                    continue;
                }

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

        //Create list of teams based on previous matchday
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
