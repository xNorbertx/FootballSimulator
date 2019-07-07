using System;
using System.Collections.Generic;
using System.Linq;
using GameBasicsSimulator.DB;
using GameBasicsSimulator.Model;

namespace GameBasicsSimulator.Service
{
    public class GenerateMatch
    {
        private double threshold = 0.75;
        private SimulatorContext _context;
       
        public GenerateMatch(SimulatorContext context) {
            _context = context;
        }

        public string Play(Team teamOne, Team teamTwo)
        {
            Match match = new Match()
            {
                MatchTeams = new List<MatchTeam>
                {
                    new MatchTeam
                    {
                        TeamId = teamOne.Id
                    },
                    new MatchTeam
                    {
                        TeamId = teamTwo.Id
                    }
                }
            };

            List<Goal> goals = new List<Goal>();

            double teamOneTotal = getOveralStrength(teamOne);
            double teamTwoTotal = getOveralStrength(teamTwo);

            double teamOneWinPercentage = teamOneTotal / teamTwoTotal;
            double teamTwoWinPercentage = teamTwoTotal / teamOneTotal;

            goals.AddRange(addGoals(teamOneWinPercentage, teamOne, teamTwo));
            goals.AddRange(addGoals(teamTwoWinPercentage, teamTwo, teamOne));

            match.Goals = goals;

            _context.Matches.Add(match);
           
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

            _context.SaveChanges();

            return teamOneGoals.Count.ToString() + "-" + teamTwoGoals.Count.ToString();
        }

        private double getOveralStrength(Team team)
        {
            return ((team.Strength * 2) + team.Morale) / 3;
        }

        private List<Goal> addGoals(double power, Team team, Team opponent)
        {
            List<Goal> goals = new List<Goal>();
            Random random = new Random();
            double luck = (double)random.Next(10000) / (double)10000;
            int time = 1;

            while (luck * power * (1 - (time*(0.5/90))) > threshold)
            {
                time = random.Next(time, 90);
                goals.Add(new Goal
                {
                    Minute = time,
                    TeamId = team.Id,
                    OpponentId = opponent.Id
                });
            }
            return goals;
        }
    }   
}
