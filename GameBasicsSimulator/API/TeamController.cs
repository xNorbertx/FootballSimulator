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
    public class TeamController : ControllerBase
    {
        private SimulatorContext _context;

        public TeamController(SimulatorContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult Get()
        {
            List<Team> teams = _context.Teams.ToList();

            if (teams.Count > 0)
            {
                foreach (Team team in teams)
                {
                    team.Goals = _context.Goals.Where(g => g.TeamId == team.Id).ToList();
                }

                return Ok(teams);
            }

            return NotFound();
        }
    }
}
