using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Simulator.Core.DTO;
using Simulator.Core.Model;
using Simulator.Infrastructure.DB;

namespace Simulator.Web.API
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
            List<SimpleMatchDto> matches = _context.Matches
                .Select(x =>
                    new SimpleMatchDto
                    {
                        TeamOne = x.TeamOne.Name,
                        TeamTwo = x.TeamTwo.Name
                    })
                .ToList();

            return Ok(matches);
        }

        [HttpPost]
        public ActionResult Play([FromBody]SimpleMatchDto model)
        {
            if (!String.IsNullOrEmpty(model.TeamOne) && !String.IsNullOrEmpty(model.TeamTwo))
            {
                return BadRequest("Oops.. no teams");
            }

            //Find teams
            List<Team> teams = _context.Teams.Where(t => t.Name == model.TeamOne ||
                                                         t.Name == model.TeamTwo).ToList();

            //Play match
            var matchResult = teams.PlayMatch();
            //Extension method for play match algorithm
            //GenerateMatch matchGenerator = new GenerateMatch(_context);
            //MatchResultDTO res = matchGenerator.Play(model.Teams[0], model.Teams[1]);

            return Ok(); 
        }

        [HttpPost]
        [Route("clear")]
        public ActionResult Clear()
        {
            //Not quite sure why cascade delete is not working here..
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
