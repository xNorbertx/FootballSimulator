using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameBasicsSimulator.DB;
using GameBasicsSimulator.Model;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GameBasicsSimulator.API
{
    [Route("api/[controller]")]
    public class TeamController : Controller
    {
        private SimulatorContext _context;

        public TeamController(SimulatorContext context)
        {
            _context = context;
        }
        // GET: api/values
        [HttpGet]
        public ActionResult<IEnumerable<Team>> Get()
        {
            List<Team> teams = _context.Teams.ToList();

            if (teams.Count > 0)
            {
                foreach (Team team in teams)
                {
                    team.Goals = _context.Goals.Where(g => g.TeamId == team.Id).ToList();
                    team.GoalsConceded = _context.Goals.Where(g => g.OpponentId == team.Id).ToList();
                    team.MatchTeams = _context.MatchTeams.Where(mt => mt.TeamId == team.Id).ToList();
                }

                return Ok(teams);
            }

            return NotFound();
        }
    }
}
