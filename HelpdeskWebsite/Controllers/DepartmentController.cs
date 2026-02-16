using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using HelpdeskDAL;
using System.Diagnostics;

namespace HelpdeskWebsite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly HelpdeskContext _context;

        public DepartmentController(HelpdeskContext context)
        {
            _context = context; 
        }

       
        [HttpGet]
        public async Task<IActionResult> GetAllDepartments()
        {
            try
            {
                
                Debug.WriteLine("GetAllDepartments endpoint hit.");

                
                List<Department> departments = await _context.Departments.ToListAsync();

                // Log the retrieved departments for debugging
                Debug.WriteLine("Departments retrieved successfully:");
                foreach (var dept in departments)
                {
                    Debug.WriteLine($"ID: {dept.Id}, DepartmentName: {dept.DepartmentName}");
                }

                
                return Ok(departments);
            }
            catch (Exception ex)
            {
                
                Debug.WriteLine($"Error in {nameof(GetAllDepartments)}: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Debug.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                Debug.WriteLine($"Stack Trace: {ex.StackTrace}");

                
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving departments.");
            }
        }
    }
}
