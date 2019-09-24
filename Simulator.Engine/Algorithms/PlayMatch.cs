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
            match.Goals = new List<Goal>();
            match.AddGoals(new List<double> { teamOneWinPercentage, teamTwoWinPercentage });
                      
            List<Goal> teamOneGoals = new List<Goal>();
            List<Goal> teamTwoGoals = new List<Goal>();
            foreach (Goal goal in match.Goals)
            {
                if (goal.TeamId == match.TeamOne.Id)
                {
                    teamOneGoals.Add(goal);
                }
                else
                {
                    teamTwoGoals.Add(goal);
                }
            }

            //Add points and alter morale stats for each team
            Team DbTeamOne = match.TeamOne;
            Team DbTeamTwo = match.TeamTwo;

            if (teamOneGoals.Count > teamTwoGoals.Count)
            {
                match.TeamOne.Points += 3;
                match.TeamOne.Morale += 2;
                match.TeamTwo.Morale -= 2;
            }
            else if (teamTwoGoals.Count > teamOneGoals.Count)
            {
                match.TeamTwo.Points += 3;
                match.TeamOne.Morale -= 2;
                match.TeamTwo.Morale += 2;
            }
            else
            {
                match.TeamOne.Points += 1;
                match.TeamTwo.Points += 1;
            }

            return match;
        }

        private static double getOveralStrength(this Team team)
        {
            return ((team.Strength * 2) + team.Morale) / 3;
        }

        private static Match AddGoals(this Match match, List<double> strength)
        {
            Random random = new Random();
            int time = 1;

            for (var i = 0; i < 2; i++)
            {
                double luck = (double)random.Next(10000) / (double)10000;

                while (luck * strength[i] * (1 - (time * (0.5 / 90))) > threshold)
                {
                    time = random.Next(time, 90);
                    match.Goals.Add(new Goal
                    {
                        Minute = time,
                        TeamId = (i == 0 ? match.TeamOne.Id : match.TeamTwo.Id)
                    });
                }
            }

            return match;
        }
    }   
}
