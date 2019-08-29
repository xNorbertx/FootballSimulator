using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameBasicsSimulator.DB;
using GameBasicsSimulator.Model;
using GameBasicsSimulator.Service;
using Microsoft.AspNetCore.Mvc;

namespace GameBasicsSimulator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchController : ControllerBase
    {
        private SimulatorContext _context;

        public MatchController(SimulatorContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult GetMatches()
        {
            List<Match> matches = _context.Matches.ToList();

            return Ok(matches);
        }

        [HttpPost]
        public ActionResult Match([FromBody]MatchDTO model)
        {
            if (model.Teams.Count != 2)
            {
                return BadRequest("Two teams are required for a match");
            }

            GenerateMatch matchGenerator = new GenerateMatch(_context);
            MatchResultDTO res = matchGenerator.Play(model.Teams[0], model.Teams[1]);

            return Ok(res); 
        }

        [HttpPost]
        [Route("clear")]
        public ActionResult Clear()
        {
            //Not quite sure why cascade delete is not working here..
            _context.MatchTeams.RemoveRange(_context.MatchTeams);
            _context.Goals.RemoveRange(_context.Goals);
            _context.Cards.RemoveRange(_context.Cards);
            _context.Matches.RemoveRange(_context.Matches);
            foreach (Team team in _context.Teams)
            {
                team.Points = 0;
            }

            _context.SaveChanges();
                
            return NoContent();
        }
    }
}
