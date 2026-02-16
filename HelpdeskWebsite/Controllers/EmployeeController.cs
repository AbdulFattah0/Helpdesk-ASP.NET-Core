using HelpdeskDAL;
using HelpdeskViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Reflection;

namespace HelpdeskWebsite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly HelpdeskContext _context;

        public EmployeeController(HelpdeskContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEmployees()
        {
            try
            {
                var viewModel = new EmployeeViewModel();
                List<EmployeeViewModel> allEmployees = await viewModel.GetAll(_context);
                return Ok(allEmployees);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error retrieving employees: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving employees.");
            }
        }

        [HttpGet("{lastname}")]
        public async Task<IActionResult> GetByLastname(string lastname)
        {
            try
            {
                var viewModel = new EmployeeViewModel { Lastname = lastname };
                await viewModel.GetByLastname(lastname, _context);
                if (viewModel.Lastname == null)
                {
                    return NotFound(new { msg = "Employee not found" });
                }
                return Ok(viewModel);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Problem in {GetType().Name}.{MethodBase.GetCurrentMethod()?.Name}: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the employee.");
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateEmployee([FromBody] EmployeeViewModel viewModel)
        {
            try
            {
                // Validate the input
                if (viewModel == null || viewModel.Id == null || viewModel.Id <= 0)
                {
                    Debug.WriteLine("Update failed: Employee ID is missing or invalid.");
                    return BadRequest(new { msg = "Employee ID is required and must be greater than zero for an update." });
                }

                // Attempt to update the employee
                var updateResult = await viewModel.Update(_context);

                // Handle the result of the update operation
                return updateResult switch
                {
                    UpdateStatus.Ok => Ok(new { msg = $"Employee {viewModel.Lastname} updated successfully!" }),
                    UpdateStatus.Stale => Conflict(new { msg = $"The data for {viewModel.Lastname} is stale. Update failed." }),
                    UpdateStatus.Failed => NotFound(new { msg = "The employee could not be found. Update failed." }),
                    _ => BadRequest(new { msg = "An unknown error occurred while updating the employee." })
                };
            }
            catch (ArgumentException ex)
            {
                Debug.WriteLine($"Validation error in {nameof(UpdateEmployee)}: {ex.Message}");
                return BadRequest(new { msg = "Validation error.", details = ex.Message });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in {nameof(UpdateEmployee)}: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new { msg = "An error occurred while updating the employee.", details = ex.Message });
            }
        }





        [HttpPost]
        public async Task<ActionResult> Post([FromBody] EmployeeViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { msg = "Invalid data provided.", errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
            }

            try
            {
                await viewModel.Add(_context);

                if (viewModel.Id > 0)
                {
                    return Ok(new { msg = $"Employee {viewModel.Lastname} added successfully!", id = viewModel.Id });
                }

                return BadRequest(new { msg = "Failed to add the employee. Please try again." });
            }
            catch (ArgumentException ex)
            {
                Debug.WriteLine($"Validation error in {GetType().Name}.{MethodBase.GetCurrentMethod()?.Name}: {ex.Message}");
                return BadRequest(new { msg = "Validation error.", details = ex.Message });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Problem in {GetType().Name}.{MethodBase.GetCurrentMethod()?.Name}: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new { msg = "An internal error occurred.", details = ex.Message });
            }
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // Retrieve the employee details before deleting
                var employee = await _context.Employees.FindAsync(id);
                if (employee == null)
                {
                    return NotFound(new { msg = "Employee not found, deletion failed!" });
                }

                // Store the last name for the message after deletion
                string lastName = employee.LastName;

                // Proceed to delete
                var viewModel = new EmployeeViewModel { Id = id };
                int result = await viewModel.Delete(id, _context);

                return result switch
                {
                    1 => Ok(new { msg = $"Employee {lastName} deleted!" }),
                    0 => NotFound(new { msg = $"Employee {lastName} not deleted!" }),
                    _ => BadRequest(new { msg = "Unknown error, deletion failed!" })
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Problem in {GetType().Name}.{MethodBase.GetCurrentMethod()?.Name}: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the employee.");
            }
        }


    }
}
