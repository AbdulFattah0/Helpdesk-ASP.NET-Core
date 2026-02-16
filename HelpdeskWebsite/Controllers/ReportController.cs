using Microsoft.AspNetCore.Mvc;
using HelpdeskWebsite.Reports;
using HelpdeskViewModels;
using HelpdeskDAL;

namespace HelpdeskWebsite.Controllers
{
    public class ReportController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly HelpdeskContext _context; 

        public ReportController(IWebHostEnvironment env, HelpdeskContext context)
        {
            _env = env;
            _context = context; 
        }

        [Route("api/employeereport")]
        [HttpGet]
        public async Task<IActionResult> GenerateReportAsync()
        {
            try
            {
                // Create an instance of EmployeeReport
                EmployeeReport hello = new();

                // Pass both rootpath and HelpdeskContext
                await hello.GenerateReportAsync(_env.WebRootPath, _context);

                // Return success response
                return Ok(new { msg = "Report Generated" });
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, new { error = "An error occurred while generating the report.", details = ex.Message });
            }
        }


        // Call Report API Endpoint
        [Route("api/callreport")]
        [HttpGet]
        public async Task<IActionResult> GenerateCallReportAsync()
        {
            try
            {
                // Log the start of the report generation process
                Console.WriteLine("Starting call report generation...");

                // Create an instance of CallReport
                CallReport callReport = new();

                // Generate the report
                await callReport.GenerateReportAsync(_env.WebRootPath, _context);

                // Log success
                Console.WriteLine("Call report successfully generated.");

                // Return success response
                return Ok(new { msg = "Call Report Generated" });
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"Error occurred while generating call report: {ex.Message}");

                // Return error response
                return StatusCode(500, new
                {
                    error = "An error occurred while generating the call report.",
                    details = ex.Message
                });
            }
        }

    }
}
