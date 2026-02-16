using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using HelpdeskDAL;
using Microsoft.EntityFrameworkCore;

namespace HelpdeskViewModels
{
    public class DepartmentViewModel
    {
        public int Id { get; set; }
        public string? DepartmentName { get; set; } = string.Empty;

        public async Task<List<DepartmentViewModel>> GetAll(HelpdeskContext context)
        {
            try
            {
                var departments = await context.Departments.ToListAsync();
                var viewModelList = new List<DepartmentViewModel>();

                foreach (var dept in departments)
                {
                    viewModelList.Add(new DepartmentViewModel
                    {
                        Id = dept.Id,
                        DepartmentName = dept.DepartmentName 
                    });
                }

                Debug.WriteLine("Successfully retrieved departments from the database.");
                return viewModelList;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in GetAll: {ex.Message}");
                throw; // Rethrow the exception to be caught by the controller
            }
        }
    }

}
