using HelpdeskDAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpdeskWebsite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProblemController : ControllerBase
    {
        private readonly HelpdeskContext _context;

        public ProblemController(HelpdeskContext context)
        {
            _context = context;
        }

        // GET: api/problem
        [HttpGet]
        public async Task<IActionResult> GetAllProblems()
        {
            try
            {
                // Query the Problems table directly
                var problems = await _context.Problems
                    .Select(p => new
                    {
                        Id = p.Id,
                        Description = p.Description
                    })
                    .ToListAsync();

                if (problems == null || !problems.Any())
                {
                    return NotFound("No problems found.");
                }

                return Ok(problems);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/problem/{description}
        [HttpGet("{description}")]
        public async Task<IActionResult> GetProblemByDescription(string description)
        {
            try
            {
                // Query the Problems table for a specific description
                var problem = await _context.Problems
                    .Where(p => p.Description == description)
                    .Select(p => new
                    {
                        Id = p.Id,
                        Description = p.Description
                    })
                    .FirstOrDefaultAsync();

                if (problem == null)
                {
                    return NotFound(new { message = $"Problem with description '{description}' not found." });
                }

                return Ok(problem);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
