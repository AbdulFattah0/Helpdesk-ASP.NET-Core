using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using HelpdeskDAL;
using System.Diagnostics;
using System.Reflection;

namespace HelpdeskDAL
{
    public class EmployeeDAO
    {
        private readonly IRepository<Employee> _repo;
        private readonly HelpdeskContext _context;

        public EmployeeDAO(HelpdeskContext context)
        {
            _repo = new HelpdeskRepository<Employee>(context);
            _context = context;
        }

        public HelpdeskContext Context => _context;

        public async Task<Employee?> GetByLastname(string lastname)
        {
            try
            {
                return await _repo.GetOne(e => e.LastName == lastname);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in {nameof(GetByLastname)}: {ex.Message}");
                return null;
            }
        }

        public async Task<Employee?> GetByEmail(string email)
        {
            try
            {
                return await _repo.GetOne(emp => emp.Email == email);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in {MethodBase.GetCurrentMethod()?.Name}: {ex.Message}");
                throw;
            }
        }

        public async Task<Employee?> GetById(int id)
        {
            try
            {
                return await _repo.GetOne(emp => emp.Id == id);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in {MethodBase.GetCurrentMethod()?.Name}: {ex.Message}");
                throw;
            }
        }

        public async Task<List<Employee>> GetAll()
        {
            try
            {
                return await _repo.GetAll();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in {MethodBase.GetCurrentMethod()?.Name}: {ex.Message}");
                throw;
            }
        }

        public async Task<Employee?> GetByPhoneNumber(string phoneNumber)
        {
            try
            {
                return await _repo.GetOne(emp => emp.PhoneNo == phoneNumber);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in {MethodBase.GetCurrentMethod()?.Name}: {ex.Message}");
                throw;
            }
        }

        public async Task<UpdateStatus> Add(Employee employee)
        {
            try
            {
                await _repo.Add(employee); // Add the entity to the repository.
                await _context.SaveChangesAsync(); // Persist the changes.

                // Ensure the ID is updated for the caller.
                _context.Entry(employee).GetDatabaseValues();

                return UpdateStatus.Ok;
            }
            catch (DbUpdateException dbEx)
            {
                Debug.WriteLine($"Database update error in {MethodBase.GetCurrentMethod()?.Name}: {dbEx.Message}");
                return UpdateStatus.Failed;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unexpected error in {MethodBase.GetCurrentMethod()?.Name}: {ex.Message}");
                return UpdateStatus.Failed;
            }
        }




        public async Task<UpdateStatus> Update(Employee employee)
        {
            try
            {
                _context.Employees.Attach(employee);
                _context.Entry(employee).State = EntityState.Modified;

                // Handle concurrency token (Timer)
                _context.Entry(employee).Property(e => e.Timer).OriginalValue = employee.Timer;

                await _context.SaveChangesAsync();
                return UpdateStatus.Ok;
            }
            catch (DbUpdateConcurrencyException)
            {
                return UpdateStatus.Stale; // Concurrency conflict
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in {nameof(Update)}: {ex.Message}");
                return UpdateStatus.Failed;
            }
        }




        public async Task<int> Delete(int id)
        {
            try
            {
                return await _repo.Delete(id);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in {MethodBase.GetCurrentMethod()?.Name}: {ex.Message}");
                throw;
            }
        }
    }
}

