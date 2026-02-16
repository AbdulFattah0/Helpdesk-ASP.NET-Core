using HelpdeskDAL;
using Microsoft.EntityFrameworkCore;
public class CallDAO
{
    private readonly HelpdeskContext _context;

    public CallDAO(HelpdeskContext context)
    {
        _context = context;
    }

    // Get all calls
    public async Task<List<Call>> GetAll()
    {
        try
        {
            return await _context.Calls.ToListAsync(); // No navigation properties included
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in CallDAO.GetAll: {ex.Message}");
            throw;
        }
    }

    // Get call by ID
    public async Task<Call?> GetById(int id)
    {
        try
        {
            return await _context.Calls.FirstOrDefaultAsync(c => c.Id == id);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in CallDAO.GetById: {ex.Message}");
            throw;
        }
    }

    // Add, Update, and Delete methods remain unchanged
}
