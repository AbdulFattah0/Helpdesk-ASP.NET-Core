using HelpdeskDAL;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
public class CallViewModel
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public int ProblemId { get; set; }
    public int TechId { get; set; }
    public string? EmployeeName { get; set; } 
    public string? ProblemDescription { get; set; } 
    public string? TechName { get; set; } 
    public DateTime DateOpened { get; set; }
    public DateTime? DateClosed { get; set; }
    public bool OpenStatus { get; set; }
    public string Notes { get; set; } = string.Empty;
   


    public string? Timer { get; set; }




    
    public async Task<CallViewModel?> GetById(HelpdeskContext context, int id)
    {
        try
        {
           
            var call = await context.Calls
                .Include(c => c.Employee) 
                .Include(c => c.Problem)  
                .Include(c => c.Tech)     
                .Where(c => c.Id == id)
                .Select(c => new CallViewModel
                {
                    Id = c.Id,
                    EmployeeId = c.EmployeeId,
                    ProblemId = c.ProblemId,
                    TechId = c.TechId,
                    EmployeeName = c.Employee.LastName, 
                    ProblemDescription = c.Problem.Description,
                    TechName = c.Tech.LastName, 
                    DateOpened = c.DateOpened,
                    DateClosed = c.DateClosed,
                    OpenStatus = c.OpenStatus,
                    Notes = c.Notes,
                    Timer = c.Timer != null ? Convert.ToBase64String(c.Timer) : null
                })
                .FirstOrDefaultAsync(); 

            return call; 
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetById: {ex.Message}");
            throw;
        }
    }


   
    public async Task<List<CallViewModel>> GetAll(HelpdeskContext context)
    {
        try
        {
            var calls = await context.Calls
                .Include(c => c.Employee)  
                .Include(c => c.Problem)   
                .Include(c => c.Tech)      
                .Select(c => new CallViewModel
                {
                    Id = c.Id,
                    EmployeeId = c.EmployeeId,
                    ProblemId = c.ProblemId,
                    TechId = c.TechId,
                    EmployeeName = c.Employee.LastName, 
                    ProblemDescription = c.Problem.Description, 
                    TechName = c.Tech != null && c.Tech.IsTech == true ? c.Tech.LastName : "N/A", 
                    DateOpened = c.DateOpened,
                    DateClosed = c.DateClosed,
                    OpenStatus = c.OpenStatus,
                    Notes = c.Notes,
                    Timer = c.Timer != null ? Convert.ToBase64String(c.Timer) : null
                })
                .ToListAsync();

            return calls;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetAll: {ex.Message}");
            throw;
        }
    }


    public async Task<bool> Delete(HelpdeskContext context, int id)
    {
        try
        {
            
            var call = await context.Calls
                .Where(c => c.Id == id)
                .Select(c => new { c.Id, c.Timer }) 
                .FirstOrDefaultAsync();

            if (call == null)
            {
                Console.WriteLine($"Call with ID {id} not found.");
                return false; 
            }

            
            var callToDelete = new Call
            {
                Id = call.Id,
                Timer = call.Timer
            };

            
            context.Calls.Attach(callToDelete);
            context.Calls.Remove(callToDelete);

           
            await context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            Console.WriteLine($"Concurrency error: Call with ID {id} may have been modified or deleted.");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in Delete: {ex.Message}");
            throw;
        }
    }


    public async Task<CallViewModel?> AddCall(HelpdeskContext context)
    {
        try
        {
        
            if (!await context.Employees.AnyAsync(e => e.Id == this.EmployeeId))
            {
                throw new Exception($"Invalid EmployeeId: {this.EmployeeId}.");
            }

            
            if (!await context.Problems.AnyAsync(p => p.Id == this.ProblemId))
            {
                throw new Exception($"Invalid ProblemId: {this.ProblemId}.");
            }

            
            if (this.TechId != 0 && !await context.Employees.AnyAsync(e => e.Id == this.TechId))
            {
                throw new Exception($"Invalid TechId: {this.TechId}.");
            }

           
            if (this.DateOpened == default)
            {
                this.DateOpened = DateTime.Now; 
            }

            
            var newCall = new Call
            {
                EmployeeId = this.EmployeeId,
                ProblemId = this.ProblemId,
                TechId = this.TechId,
                DateOpened = this.DateOpened,
                Notes = string.IsNullOrWhiteSpace(this.Notes) ? string.Empty : this.Notes.Substring(0, Math.Min(this.Notes.Length, 250)),
                OpenStatus = true 
            };

            
            context.Calls.Add(newCall);

            
            await context.SaveChangesAsync();

           
            if (this.TechId != 0)
            {
                var tech = await context.Employees.FirstOrDefaultAsync(e => e.Id == this.TechId);
                if (tech != null)
                {
                    tech.IsTech = true;
                    context.Entry(tech).State = EntityState.Modified;
                    await context.SaveChangesAsync();
                }
            }

            
            var savedCall = await context.Calls
                .Include(c => c.Employee) 
                .Include(c => c.Problem)  
                .Include(c => c.Tech)     
                .FirstOrDefaultAsync(c => c.Id == newCall.Id);

            if (savedCall == null)
            {
                throw new Exception("Failed to retrieve the saved call with related data.");
            }

            
            return new CallViewModel
            {
                Id = savedCall.Id,
                EmployeeId = savedCall.EmployeeId,
                ProblemId = savedCall.ProblemId,
                TechId = savedCall.TechId,
                EmployeeName = savedCall.Employee?.LastName, 
                ProblemDescription = savedCall.Problem?.Description, 
                TechName = savedCall.Tech?.LastName, 
                DateOpened = savedCall.DateOpened,
                DateClosed = savedCall.DateClosed,
                OpenStatus = savedCall.OpenStatus,
                Notes = savedCall.Notes,
                Timer = savedCall.Timer != null ? Convert.ToBase64String(savedCall.Timer) : null
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in AddCall: {ex.Message}");
            throw;
        }
    }



    public async Task<CallViewModel?> UpdateCall(HelpdeskContext context)
    {
        try
        {
            
            var existingCall = await context.Calls
                .Include(c => c.Tech) 
                .FirstOrDefaultAsync(c => c.Id == this.Id);

            if (existingCall == null)
            {
                throw new Exception($"Call with ID {this.Id} not found.");
            }

            
            if (existingCall.TechId != this.TechId && existingCall.TechId != 0)
            {
                var oldTech = await context.Employees.FirstOrDefaultAsync(e => e.Id == existingCall.TechId);
                if (oldTech != null)
                {
                    
                    bool isStillTech = await context.Calls.AnyAsync(c => c.TechId == oldTech.Id && c.Id != this.Id);
                    if (!isStillTech)
                    {
                        oldTech.IsTech = false;
                        context.Entry(oldTech).State = EntityState.Modified;
                    }
                }
            }

            
            if (this.TechId != 0)
            {
                var newTech = await context.Employees.FirstOrDefaultAsync(e => e.Id == this.TechId);
                if (newTech != null)
                {
                    newTech.IsTech = true;
                    context.Entry(newTech).State = EntityState.Modified;
                }
            }

          
            existingCall.EmployeeId = this.EmployeeId;
            existingCall.ProblemId = this.ProblemId;
            existingCall.TechId = this.TechId;
            existingCall.DateOpened = this.DateOpened;
            existingCall.DateClosed = this.DateClosed;
            existingCall.Notes = this.Notes;
            existingCall.OpenStatus = this.OpenStatus;

         
            await context.SaveChangesAsync();

            return this;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in UpdateCall: {ex.Message}");
            throw;
        }
    }





}

