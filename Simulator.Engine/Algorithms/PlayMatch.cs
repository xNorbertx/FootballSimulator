using System;
using System.Collections.Generic;
using System.Linq;
using Simulator.Core.DTO;
using Simulator.Core.Model;

namespace Simulator.Engine.Algorithms
{
    public static class Play
    {
        private static double threshold = 0.65;
       
        public static Match PlayMatch(this Match match)
        {
            //Create a match
            List<Goal> goals = new List<Goal>();

            //Determine strength indicator of teams based on stats
            double teamOneTotal = match.TeamOne.getOveralStrength();
            double teamTwoTotal = match.TeamTwo.getOveralStrength();

            double teamOneWinPercentage = teamOneTotal / teamTwoTotal;
            double teamTwoWinPercentage = teamTwoTotal / teamOneTotal;

            //Add goals for each team
            match.AddGoals(teamOneWinPercentage, teamTwoWinPercentage);
                      
            List<Goal> teamOneGoals = new List<Goal>();
            List<Goal> teamTwoGoals = new List<Goal>();
            foreach (Goal goal in match.Goals)
            {
                if (goal.TeamId == teamOne.Id)
                {
                    teamOneGoals.Add(goal);
                }
                else
                {
                    teamTwoGoals.Add(goal);
                }
            }

            //Add points and alter morale stats for each team
            Team DbTeamOne = _context.Teams.Where(t => t.Id == teamOne.Id).First();
            Team DbTeamTwo = _context.Teams.Where(t => t.Id == teamTwo.Id).First();

            if (teamOneGoals.Count > teamTwoGoals.Count)
            {
                DbTeamOne.Points += 3;
                DbTeamOne.Morale += 2;
                DbTeamTwo.Morale -= 2;
            }
            else if (teamTwoGoals.Count > teamOneGoals.Count)
            {
                DbTeamTwo.Points += 3;
                DbTeamOne.Morale -= 2;
                DbTeamTwo.Morale += 2;
            }
            else
            {
                DbTeamOne.Points += 1;
                DbTeamTwo.Points += 1;
            }

            _context.Matches.Add(match);
            _context.SaveChanges();

            return new Match
            {
                Score = teamOneGoals.Count.ToString() + "-" + teamTwoGoals.Count.ToString()
            };
        }

        private static double getOveralStrength(this Team team)
        {
            return ((team.Strength * 2) + team.Morale) / 3;
        }

        private static List<Goal> addGoals(this Match match, double strengthOne, double strengthTwo)
        {
            List<Goal> goals = new List<Goal>();
            Random random = new Random();
            double luck = (double)random.Next(10000) / (double)10000;
            int time = 1;

            for (var i = 0; i < 2; i++)
            {
                while (luck * strength * (1 - (time * (0.5 / 90))) > threshold)
                {
                    time = random.Next(time, 90);
                    goals.Add(new Goal
                    {
                        Minute = time,
                        TeamId = team.Id
                    });
                }
            }
            //The algorithm which decides the amount of goals, based on luck and strength
            
            return goals;
        }
    }   
}
