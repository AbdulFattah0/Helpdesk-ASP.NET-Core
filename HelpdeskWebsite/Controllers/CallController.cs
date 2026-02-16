using Microsoft.AspNetCore.Mvc;
using HelpdeskViewModels;
using HelpdeskDAL;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace HelpdeskWebsite.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CallController : Controller
    {
        private readonly HelpdeskContext _context;

        public CallController(HelpdeskContext context)
        {
            _context = context;
        }

        // GET: api/call
        [HttpGet]
        public async Task<IActionResult> GetAllCalls()
        {
            var callViewModel = new CallViewModel();
            var calls = await callViewModel.GetAll(_context); // Pass context here

            if (calls == null || calls.Count == 0)
            {
                return NotFound("No calls found.");
            }
            return Ok(calls);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCallById(int id)
        {
            var callViewModel = new CallViewModel();
            var call = await callViewModel.GetById(_context, id); // Adjust based on your actual code logic

            if (call == null)
            {
                return NotFound($"Call with ID {id} not found.");
            }

            return Ok(call);
        }

        // DELETE: api/call/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCall(int id)
        {
            var callViewModel = new CallViewModel();

            try
            {
                var result = await callViewModel.Delete(_context, id);

                if (result)
                {
                    return Ok($"Call with ID {id} successfully deleted.");
                }
                else
                {
                    return NotFound($"Call with ID {id} not found.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while trying to delete the call: {ex.Message}");
            }
        }



        [HttpPost]
        public async Task<IActionResult> AddCall([FromBody] CallViewModel callViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid data.");
            }

            try
            {
                var addedCall = await callViewModel.AddCall(_context);

                if (addedCall != null)
                {
                    return CreatedAtAction(nameof(GetCallById), new { id = addedCall.Id }, addedCall);
                }
                else
                {
                    return BadRequest("Failed to add the call.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while adding the call: {ex.Message}");
            }
        }


        // PUT: api/call/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCall(int id, [FromBody] CallViewModel callViewModel)
        {
            if (id != callViewModel.Id)
            {
                return BadRequest("Call ID mismatch.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid data.");
            }

            try
            {
                var updatedCall = await callViewModel.UpdateCall(_context);

                if (updatedCall == null)
                {
                    return NotFound($"Call with ID {id} not found.");
                }

                return Ok(updatedCall);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict("Concurrency error occurred while updating the call.");
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while updating the call: {ex.Message}");
            }
        }




    }
}
    






