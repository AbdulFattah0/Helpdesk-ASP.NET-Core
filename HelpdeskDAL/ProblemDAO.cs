using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpdeskDAL
{
    public class ProblemDAO
    {
        private readonly HelpdeskContext _context;

        public ProblemDAO(DbContextOptions<HelpdeskContext> options)
        {
            _context = new HelpdeskContext(options);
        }

        public Problem GetByDescription(string description)
        {
            try
            {
                var problem = _context.Problems.FirstOrDefault(p => p.Description == description);
                if (problem == null)
                {
                    throw new Exception($"Problem with description '{description}' not found.");
                }
                return problem;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ProblemDAO.GetByDescription: {ex.Message}");
                throw new Exception("Error retrieving problem by description", ex);
            }
        }

        public List<Problem> GetAll()
        {
            try
            {
                if (_context.Problems == null)
                {
                    throw new Exception("Problems table is not initialized in the database context.");
                }

                return _context.Problems.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ProblemDAO.GetAll: {ex.Message}");
                throw new Exception("Error retrieving all problems", ex);
            }
        }
    }

}
