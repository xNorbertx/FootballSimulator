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

        [HttpPost]
        public ActionResult<MatchResultDTO> Match([FromBody]MatchDTO model)
        {
            if (model.Teams.Count != 2)
            {
                return BadRequest("Two teams are required for a match");
            }

            GenerateMatch matchGenerator = new GenerateMatch(_context);
            MatchResultDTO res = matchGenerator.Play(model.Teams[0], model.Teams[1]);

            return res; 
        }
    }
}
